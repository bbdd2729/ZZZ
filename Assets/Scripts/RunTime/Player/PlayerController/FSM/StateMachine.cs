using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class StateMachine : IStateMachine
{
    public StateMachine(PlayerController playerController) // 状态机构造函数
    {
        _playerController = playerController;
        _characterController = playerController._characterController;
        _animator = playerController._animator;
        PerformanceMonitor = new StateMachinePerformanceMonitor();
    }

    public void RegisterState<T>(T state) where T : IState
    {
        var type = typeof(T);

        if (_states.ContainsKey(type))
        {
            Debug.LogWarning($"State of type {type} is already registered.");
            return;
        }

        // 如果状态是 BaseState 的子类，则初始化它
        if (state is BaseState baseState) baseState.Initialize(this);

        _states[type] = state;
    }

    public void ChangeState<T>() where T : IState
    {
        if (StateLocked || !IsEnabled) return;

        var type = typeof(T);
        if (_states.TryGetValue(type, out var newState))
        {
            _currentState?.OnExit();  // 退出当前状态
            _currentState = newState; // 设置新状态
            _currentState.OnEnter();  // 进入新状态
        }
        else
        {
            Debug.LogError($"State {type} not registered!");
        }
    }

    public void Update()
    {
        if (!IsEnabled) return; // 禁用时停止全部逻辑

        // 使用性能监控记录状态更新
        PerformanceMonitor?.RecordStateUpdate(_currentState, () => { _currentState?.Update(); });
    }

    public void Lock()
    {
        StateLocked = true;
    }

    public void Unlock()
    {
        StateLocked = false;
    }

    /*public void Dispose()
    {
        _currentState?.OnExit();
        _currentState?.Dispose();

        foreach (var state in _states.Values)
        {
            state.Dispose();
        }

        _disposables?.Dispose();
        _states.Clear();
    }*/
    private void OnDestroy()
    {
        //Dispose();
    }

    public void Dispose() { }

    public void Enable()
    {
        if (IsEnabled) return;
        IsEnabled = true;

        // 不在这里控制输入,由外部或状态自己控制
        // _playerController.SetInputActive(true);

        // 重新进入当前状态(可选)
        // _currentState?.OnEnter();
    }

    public void Disable()
    {
        if (!IsEnabled) return;
        IsEnabled = false;

        // 不在这里控制输入,由外部或状态自己控制
        // _playerController.SetInputActive(false);

        // 可选:让当前状态暂停(如需要)
        // _currentState?.OnExit();
    }

    #region 字段定义

    internal PlayerController         _playerController;                //角色控制器
    internal CharacterController      _characterController;             //角色控制器
    internal Animator                 _animator;                        //角色动画器
    private  IState                   _currentState;                    //状态接口
    internal Dictionary<Type, IState> _states                  = new(); //状态字典
    private  CompositeDisposable      _disposables             = new(); //状态机使用的可取消订阅
    internal int                      currentNormalAttackIndex = 1;     //当前普通攻击索引

    public bool IsEnabled { get; private set; } = true;

    #region 状态机锁

    // 为内部状态类提供访问_stateLocked的公共方法
    public bool GetStateLocked()
    {
        return StateLocked;
    }

    public void SetStateLocked(bool value)
    {
        StateLocked = value;
    }

    public BaseState CurrentState
    {
        get => _currentState as BaseState;
        private set => _currentState = value;
    }

    public bool StateLocked { get; set; }

    #endregion

    // 性能监控

    public StateMachinePerformanceMonitor PerformanceMonitor { get; }

    #endregion
}