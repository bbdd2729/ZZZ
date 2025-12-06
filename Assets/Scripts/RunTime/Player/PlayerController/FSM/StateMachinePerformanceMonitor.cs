using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class StateMachinePerformanceMonitor
{
    private readonly Stopwatch _stateUpdateTimer = new();
    private readonly Dictionary<Type, long> _stateUpdateTimes = new();
    private readonly Dictionary<Type, int> _stateUpdateCounts = new();
    private long _totalUpdateTime;
    private int _totalUpdateCount;

    public void RecordStateUpdate(IState state, Action updateAction)
    {
        if (state == null)
        {
            updateAction?.Invoke();
            return;
        }

        _stateUpdateTimer.Restart();
        updateAction?.Invoke();
        _stateUpdateTimer.Stop();

        var stateType = state.GetType();
        var elapsedMs = _stateUpdateTimer.ElapsedMilliseconds;

        // 记录状态更新时间
        if (_stateUpdateTimes.ContainsKey(stateType))
        {
            _stateUpdateTimes[stateType] += elapsedMs;
            _stateUpdateCounts[stateType]++;
        }
        else
        {
            _stateUpdateTimes[stateType] = elapsedMs;
            _stateUpdateCounts[stateType] = 1;
        }

        _totalUpdateTime += elapsedMs;
        _totalUpdateCount++;

        // 性能警告 - 超过16ms（1帧时间）
        if (elapsedMs > 16)
        {
            Debug.LogWarning($"State {stateType.Name} took {elapsedMs}ms to update");
        }
    }

    public Dictionary<Type, long> GetAverageUpdateTimes()
    {
        var averages = new Dictionary<Type, long>();
        foreach (var kvp in _stateUpdateTimes)
        {
            var stateType = kvp.Key;
            var totalTime = kvp.Value;
            var count = _stateUpdateCounts[stateType];
            averages[stateType] = count > 0 ? totalTime / count : 0;
        }
        return averages;
    }

    public long GetTotalAverageUpdateTime()
    {
        return _totalUpdateCount > 0 ? _totalUpdateTime / _totalUpdateCount : 0;
    }

    public void Reset()
    {
        _stateUpdateTimes.Clear();
        _stateUpdateCounts.Clear();
        _totalUpdateTime = 0;
        _totalUpdateCount = 0;
    }

    public void LogPerformanceReport()
    {
        Debug.Log("=== State Machine Performance Report ===");
        
        var averages = GetAverageUpdateTimes();
        foreach (var kvp in averages)
        {
            Debug.Log($"State: {kvp.Key.Name}, Avg Time: {kvp.Value}ms, " +
                      $"Total Calls: {_stateUpdateCounts[kvp.Key]}");
        }
        
        Debug.Log($"Total Average Update Time: {GetTotalAverageUpdateTime()}ms");
        Debug.Log("=====================================");
    }
}