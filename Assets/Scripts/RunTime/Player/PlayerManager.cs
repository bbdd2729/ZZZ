using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

public enum PlayerName
    {
        Anbi,
        Coein,
        Nike,
    }


public class PlayerManager : SingletonBase<PlayerManager>, IPlayerManager
{
    [ShowInInspector] private int                    _currentPlayerIndex = 0;
    [ShowInInspector] public  List<PlayerController> PlayerControllers   = new List<PlayerController>();
    public                    PlayerController       CurrentPlayer { get; set; }
    
    // 显式实现接口属性
    IReadOnlyList<PlayerController> IPlayerManager.PlayerControllers => PlayerControllers;
    
    // 依赖注入新的切换管理器
    [Inject] private IPlayerSwitchManager _switchManager;
    
    // 添加事件支持
    public event Action<PlayerController> OnPlayerSwitched;
    
    // 功能开关 - 用于回滚机制
    [SerializeField] private bool _useNewSwitchSystem = true;

    public void Init()
    {
        if (PlayerControllers == null)
            PlayerControllers = new List<PlayerController>();

        PlayerControllers.Clear();
        _currentPlayerIndex = 0;
        CurrentPlayer = null;
        DebugX.Instance.Log("PlayerManager 初始化完成");
        InputSystem.Instance.SwitchCharacterEvent += _ => SwitchNextPlayer();
    }

    public void AddPlayer(PlayerController playerController)
    {
        PlayerControllers.Add(playerController);
    }

    public void SwitchNextPlayer()
    {
        if (!CanSwitchPlayer()) return;
        
        if (_useNewSwitchSystem && _switchManager != null && !_switchManager.IsSwitching)
        {
            // 使用新的切换系统
            PerformNewPlayerSwitch();
        }
        else
        {
            // 回退到原有切换逻辑
            PerformLegacyPlayerSwitch();
        }
    }
    
    private void PerformNewPlayerSwitch()
    {
        var oldPlayer = CurrentPlayer;
        _currentPlayerIndex = (_currentPlayerIndex + 1) % PlayerControllers.Count;
        var newPlayer = PlayerControllers[_currentPlayerIndex];
        
        // 使用新的切换管理器
        _switchManager.StartPlayerSwitch(oldPlayer, newPlayer);
        CurrentPlayer = newPlayer;
        
        // 触发切换事件
        OnPlayerSwitched?.Invoke(CurrentPlayer);
    }
    
    private void PerformLegacyPlayerSwitch()
    {
        // 禁用当前角色
        if (CurrentPlayer != null)
        {
            // 立即禁用输入和组件
            CurrentPlayer.SetInputActive(false);
            CurrentPlayer.enabled = false;
            CurrentPlayer._stateMachine.ChangeState<SwitchOutState>();
        }
        
        _currentPlayerIndex = (_currentPlayerIndex + 1) % PlayerControllers.Count;
        var oldPlayer = CurrentPlayer;
        CurrentPlayer = PlayerControllers[_currentPlayerIndex];
        
        // 启用新角色，但先不启用输入，等待 SwitchInState 结束后再启用
        CurrentPlayer.gameObject.SetActive(true);
        CurrentPlayer.enabled = true; // 先启用组件，但输入控制由状态管理
        CurrentPlayer._stateMachine.ChangeState<SwitchInState>();
        
        // 在新角色切换入后，隐藏旧角色
        if (oldPlayer != null)
        {
            oldPlayer.gameObject.SetActive(false);
        }
        
        // 触发切换事件
        OnPlayerSwitched?.Invoke(CurrentPlayer);
    }

    public bool CanSwitchPlayer()
    {
        return CurrentPlayer != null && !CurrentPlayer._stateMachine.StateLocked;
    }

    public void Initialize()
    {
        Init();
    }

    public void SwitchToPlayer(int playerIndex)
    {
        if (!CanSwitchPlayer()) return;
        
        if (_useNewSwitchSystem && _switchManager != null && !_switchManager.IsSwitching)
        {
            // 使用新的切换系统
            PerformNewPlayerSwitchTo(playerIndex);
        }
        else
        {
            // 回退到原有切换逻辑
            PerformLegacyPlayerSwitchTo(playerIndex);
        }
    }
    
    private void PerformNewPlayerSwitchTo(int playerIndex)
    {
        var oldPlayer = CurrentPlayer;
        var newPlayer = PlayerControllers[playerIndex];
        
        // 使用新的切换管理器
        _switchManager.StartPlayerSwitch(oldPlayer, newPlayer);
        _currentPlayerIndex = playerIndex;
        CurrentPlayer = newPlayer;
        
        // 触发切换事件
        OnPlayerSwitched?.Invoke(CurrentPlayer);
    }
    
    private void PerformLegacyPlayerSwitchTo(int playerIndex)
    {
        
        // 禁用当前角色
        if (CurrentPlayer != null)
        {
            // 立即禁用输入和组件
            CurrentPlayer.SetInputActive(false);
            CurrentPlayer.enabled = false;
            CurrentPlayer._stateMachine.ChangeState<SwitchOutState>();
        }
        
        var oldPlayerIndex = _currentPlayerIndex;
        var oldPlayer = CurrentPlayer;
        _currentPlayerIndex = playerIndex;
        CurrentPlayer = PlayerControllers[_currentPlayerIndex];
        
        // 启用新角色，但先不启用输入，等待 SwitchInState 结束后再启用
        CurrentPlayer.gameObject.SetActive(true);
        CurrentPlayer.enabled = true; // 先启用组件，但输入控制由状态管理
        CurrentPlayer._stateMachine.ChangeState<SwitchInState>();
        
        // 在新角色切换入后，隐藏旧角色
        if (oldPlayerIndex != _currentPlayerIndex && oldPlayer != null)
        {
            oldPlayer.gameObject.SetActive(false);
        }
        
        // 触发切换事件
        OnPlayerSwitched?.Invoke(CurrentPlayer);
    }

    public void LoadPlayer()
    {
        CurrentPlayer = PlayerControllers[_currentPlayerIndex];
        
        for (int i = 0; i < PlayerControllers.Count; i++)
        {
            var playerController = PlayerControllers[i];
            if (i == _currentPlayerIndex)
            {
                playerController.gameObject.SetActive(true);
                playerController.enabled = true;
                playerController.SetInputActive(true);
                playerController._stateMachine.ChangeState<IdleState>();
            }
            else
            {
                playerController.gameObject.SetActive(false); // 非当前角色隐藏
                playerController.enabled = false;
                playerController.SetInputActive(false);
            }
        }
    }
}