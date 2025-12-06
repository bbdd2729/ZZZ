using System.Collections;
using UnityEngine;

public interface IPlayerSwitchManager
{
    bool IsSwitching { get; }
    void StartPlayerSwitch(PlayerController fromPlayer, PlayerController toPlayer);
}

public class PlayerSwitchManager : IPlayerSwitchManager
{
    private readonly IEventBus _eventBus;
    
    private PlayerSwitchContext _currentSwitchContext;
    private bool _isSwitching = false;
    
    public PlayerSwitchManager(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public bool IsSwitching => _isSwitching;
    
    public void StartPlayerSwitch(PlayerController fromPlayer, PlayerController toPlayer)
    {
        if (_isSwitching) return;
        
        _currentSwitchContext = new PlayerSwitchContext
        {
            FromPlayer = fromPlayer,
            ToPlayer = toPlayer,
            StartTime = Time.time
        };
        
        // 使用Unity的MonoBehaviour来启动协程
        var coroutineRunner = new GameObject("PlayerSwitchCoroutine").AddComponent<CoroutineRunner>();
        coroutineRunner.StartCoroutine(PerformPlayerSwitch(coroutineRunner));
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator PerformPlayerSwitch(CoroutineRunner coroutineRunner)
    {
        _isSwitching = true;
        DebugX.Instance.Log("Player switch started");
        
        // 阶段1：准备切换
        yield return PrepareSwitch();
        
        // 阶段2：执行切换出
        yield return SwitchOut();
        
        // 阶段3：执行切换入
        yield return SwitchIn();
        
        // 阶段4：完成切换
        yield return CompleteSwitch();
        
        _isSwitching = false;
        DebugX.Instance.Log("Player switch completed");
        
        // 发布切换完成事件
        _eventBus?.Publish(new PlayerSwitchCompletedEvent(
            _currentSwitchContext.FromPlayer,
            _currentSwitchContext.ToPlayer
        ));
        
        // 清理协程运行器
        GameObject.Destroy(coroutineRunner.gameObject);
    }
    
    private IEnumerator PrepareSwitch()
    {
        var fromPlayer = _currentSwitchContext.FromPlayer;
        
        // 锁定当前玩家的状态机
        fromPlayer._stateMachine.Lock();
        
        // 检查是否可以切换 - 使用StateLocked属性
        if (fromPlayer._stateMachine is StateMachine stateMachine && stateMachine.StateLocked)
        {
            DebugX.Instance.LogWarning("Cannot switch player - current state locked");
            yield break;
        }
        
        yield return null;
    }
    
    private IEnumerator SwitchOut()
    {
        var fromPlayer = _currentSwitchContext.FromPlayer;
        
        // 切换到切换出状态
        fromPlayer._stateMachine.ChangeState<SwitchOutState>();
        
        // 等待切换出动画完成
        yield return WaitForSwitchAnimation("SwitchOut_Normal", fromPlayer);
        
        // 禁用当前玩家
        fromPlayer.SetInputActive(false);
        fromPlayer.enabled = false;
    }
    
    private IEnumerator SwitchIn()
    {
        var toPlayer = _currentSwitchContext.ToPlayer;
        
        // 激活目标玩家
        toPlayer.gameObject.SetActive(true);
        toPlayer.enabled = true;
        
        // 切换到切换入状态
        toPlayer._stateMachine.ChangeState<SwitchInState>();
        
        // 等待切换入动画完成
        yield return WaitForSwitchAnimation("SwitchIn_Normal", toPlayer);
    }
    
    private IEnumerator CompleteSwitch()
    {
        var toPlayer = _currentSwitchContext.ToPlayer;
        
        // 启用新玩家的输入
        toPlayer.SetInputActive(true);
        
        // 解锁状态机
        toPlayer._stateMachine.Unlock();
        
        // 隐藏旧玩家
        _currentSwitchContext.FromPlayer.gameObject.SetActive(false);
        
        yield return null;
    }
    
    private IEnumerator WaitForSwitchAnimation(string animationName, PlayerController player)
    {
        var animator = player.GetComponent<Animator>();
        var waitTime = 0f;
        var maxWaitTime = 2f; // 最大等待时间
        
        while (waitTime < maxWaitTime)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                yield break;
            }
            
            waitTime += Time.deltaTime;
            yield return null;
        }
        
        DebugX.Instance.LogWarning($"Animation {animationName} did not complete in time");
    }
}

// 协程运行器辅助类
public class CoroutineRunner : MonoBehaviour
{
    // 这个类仅用于运行协程
}

// 切换上下文
public class PlayerSwitchContext
{
    public PlayerController FromPlayer { get; set; }
    public PlayerController ToPlayer { get; set; }
    public float StartTime { get; set; }
}

// 切换完成事件
public class PlayerSwitchCompletedEvent
{
    public PlayerController FromPlayer { get; }
    public PlayerController ToPlayer { get; }
    
    public PlayerSwitchCompletedEvent(PlayerController fromPlayer, PlayerController toPlayer)
    {
        FromPlayer = fromPlayer;
        ToPlayer = toPlayer;
    }
}