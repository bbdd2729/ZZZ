using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class StateMachine : IStateMachine
{
    internal Animator                 _animator;
    internal CharacterController      _characterController;
    private  IState                   _currentState;
    private  CompositeDisposable      _disposables = new();
    internal PlayerController         _playerController;
    internal Dictionary<Type, IState> _states                  = new();
    internal int                      currentNormalAttackIndex = 1;
    private  bool                     _isEnabled               = true;
    
    // 为内部状态类提供访问_stateLocked的公共方法
    public bool GetStateLocked() => StateLocked;
    public void SetStateLocked(bool value) => StateLocked = value;
    
    // 实现IStateMachine接口的属性
    public BaseState CurrentState
    {
        get => _currentState as BaseState;
        private set => _currentState = value;
    }
    
    // 实现IStateMachine接口的StateLocked属性
    public bool StateLocked { get; set; } = false;

    // 性能监控
    private StateMachinePerformanceMonitor _performanceMonitor;
    
    public StateMachinePerformanceMonitor PerformanceMonitor => _performanceMonitor;

    public StateMachine(PlayerController playerController, CharacterController characterController, Animator animator) // 状态机构造函数
    {
        _playerController = playerController;
        _characterController = characterController;
        _animator = animator;
        _performanceMonitor = new StateMachinePerformanceMonitor();
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
        if (StateLocked || !_isEnabled) return;

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
        if (!_isEnabled) return;  // 禁用时停止全部逻辑
        
        // 使用性能监控记录状态更新
        _performanceMonitor?.RecordStateUpdate(_currentState, () =>
        {
            _currentState?.Update();
        });
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
        if (_isEnabled) return;
        _isEnabled = true;

        // 不在这里控制输入,由外部或状态自己控制
        // _playerController.SetInputActive(true);

        // 重新进入当前状态(可选)
        // _currentState?.OnEnter();
    }

    public void Disable()
    {
        if (!_isEnabled) return;
        _isEnabled = false;

        // 不在这里控制输入,由外部或状态自己控制
        // _playerController.SetInputActive(false);

        // 可选:让当前状态暂停(如需要)
        // _currentState?.OnExit();
    }
    
    // 添加一个属性来检查状态机是否启用
    public bool IsEnabled => _isEnabled;
    
    // 实现IStateMachine接口的方法
    public void Lock()
    {
        StateLocked = true;
    }
    
    public void Unlock()
    {
        StateLocked = false;
    }
}