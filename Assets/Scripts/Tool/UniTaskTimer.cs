using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

/// <summary>
///     通用计时器。全程 UniTask 实现，0 GC。
/// </summary>
public sealed class UniTaskTimer : IDisposable
{
    public enum Mode
    {
        Once, // 到点回调一次后自动停止
        Loop  // 到点回调后重新开始，直到手动 Stop 或取消
    }

    public enum TimeSource
    {
        Scaled,   // Time.time
        Unscaled, // Time.unscaledTime
        Fixed     // Time.fixedTime
    }

    private readonly Action                  _callback;
    private readonly CancellationTokenSource _cts = new();
    private readonly double                  _interval;

    private readonly Mode                            _mode;
    private readonly TimeSource                      _source;
    private          IUniTaskAsyncEnumerable<object> _core;

    private double _startTime;

    private UniTaskTimer(Mode mode,
                         double interval,
                         TimeSource source,
                         Action callback)
    {
        _mode = mode;
        _interval = interval;
        _source = source;
        _callback = callback;
    }

    /// <summary>
    ///     当前是否正在运行
    /// </summary>
    public bool IsRunning => _core != null;

    /// <summary>
    ///     已经完成的周期数（单次模式里只有 0/1）
    /// </summary>
    public long ElapsedCycles { get; private set; }

    /// <summary>
    ///     剩余时间（秒）
    /// </summary>
    public double RemainingTime
    {
        get
        {
            if (!IsRunning) return 0;
            var interval = _interval;
            var elapsed = GetTime() - _startTime;
            return Math.Max(0, interval - elapsed);
        }
    }

    /// <summary>
    ///     释放资源，停止计时器
    /// </summary>
    public void Dispose()
    {
        Stop();
        _cts.Dispose();
    }

    /// <summary>
    ///     创建并立即启动一个计时器
    /// </summary>
    public static UniTaskTimer Start(Mode mode,
                                     double interval,
                                     TimeSource source = TimeSource.Scaled,
                                     Action callback = null)
    {
        var t = new UniTaskTimer(mode, interval, source, callback);
        t.StartCore();
        return t;
    }

    /// <summary>
    ///     手动停止计时器（可再 Start）
    /// </summary>
    public void Stop()
    {
        if (!IsRunning) return;
        _cts.Cancel(); // 会触发 Dispose
    }

    /* -------------------------------------------------------------------- */

    private void StartCore()
    {
        if (IsRunning) return;
        _startTime = GetTime();
        ElapsedCycles = 0;
        _core = RunCore();
        // Fire-and-forget，内部自己管理生命周期
        RunAsync().Forget();
    }

    private async UniTaskVoid RunAsync()
    {
        try
        {
            await foreach (var _ in _core.WithCancellation(_cts.Token)) { }
        }
        catch (OperationCanceledException)
        {
            /* 预期 */
        }
        finally
        {
            _core = null;
        }
    }

    private IUniTaskAsyncEnumerable<object> RunCore()
    {
        return UniTaskAsyncEnumerable.Create<object>(async (writer, token) =>
        {
            while (!token.IsCancellationRequested)
            {
                var target = _startTime + _interval;
                await WaitUntil(target, token);
                if (token.IsCancellationRequested) break;

                ElapsedCycles++;
                try
                {
                    _callback?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                if (_mode == Mode.Once) break;

                _startTime = GetTime(); // 下一轮
            }
        });
    }

    private async UniTask WaitUntil(double targetTime, CancellationToken token)
    {
        switch (_source)
        {
            case TimeSource.Scaled:
                await UniTask.WaitUntil(() => GetTime() >= targetTime,
                                        PlayerLoopTiming.Update, token);
                break;
            case TimeSource.Unscaled:
                await UniTask.WaitUntil(() => GetTime() >= targetTime,
                                        PlayerLoopTiming.Update, token);
                break;
            case TimeSource.Fixed:
                await UniTask.WaitUntil(() => GetTime() >= targetTime,
                                        PlayerLoopTiming.FixedUpdate, token);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetTime()
    {
        return _source switch
        {
            TimeSource.Scaled   => Time.time,
            TimeSource.Unscaled => Time.unscaledTime,
            TimeSource.Fixed    => Time.fixedTime,
            _                   => throw new ArgumentOutOfRangeException()
        };
    }
}