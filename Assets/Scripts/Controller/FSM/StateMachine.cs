using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class StateMachine
{
    internal Animator                 _animator;
    internal CharacterController      _characterController;
    private  IState                   _currentState;
    private  CompositeDisposable      _disposables = new();
    internal PlayerController         _playerController;
    internal Dictionary<Type, IState> _states                  = new();
    internal bool                     StateLocked              = false;
    internal int                      currentNormalAttackIndex = 1;

    public StateMachine(PlayerController playerController, CharacterController characterController, Animator animator) // 状态机构造函数
    {
        _playerController = playerController;
        _characterController = characterController;
        _animator = animator;
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
        if (StateLocked) return;

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
        _currentState?.Update();
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
}