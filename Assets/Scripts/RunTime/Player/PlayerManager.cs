using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum PlayerName
{
    Anbi,
    Coein,
    Nike
}


public class PlayerManager : SingletonBase<PlayerManager>
{
    [ShowInInspector] private int                    _currentPlayerIndex;       // 当前角色索引
    [ShowInInspector] public  List<PlayerController> PlayerControllers = new(); // 角色控制器列表
    [ShowInInspector] public  List<GameObject>       PlayerInstances   = new(); // 角色模型列表

    [ShowInInspector]
    public PlayerController CurrentPlayer
    {
        get => PlayerControllers[_currentPlayerIndex];
    }
    //[ShowInInspector] public  bool                   CanSwitchPlayer { get =>  }


    public static event Action<PlayerController> OnPlayerSwitched; // 添加事件支持

    public void Init()
    {
        Debug.Log("PlayerManager Init");
        InputSystem.Instance.SwitchCharacterEvent += ctx => SwitchToNextPlayer();
    } // 初始化

    public void SwitchToPlayer(int playerIndex) // 切换到指定角色
    {
        if (playerIndex < 0 || playerIndex >= PlayerControllers.Count)
        {
            Debug.LogWarning($"[CharacterManager] 非法索引 {playerIndex}");
            return;
        }

        if (playerIndex == _currentPlayerIndex) return;
        // 1. 销毁旧角色（或休眠）
        PlayerExit();
        // 2. 生成新角色（或激活）
        PlayerEnter(playerIndex);

        _currentPlayerIndex = playerIndex;
        OnPlayerSwitched?.Invoke(PlayerControllers[playerIndex]);
    }

    public void SwitchToNextPlayer() // 切换到下个角色
    {
        if (PlayerControllers.Count == 0) return;
        var nextIndex = (_currentPlayerIndex + 1) % PlayerControllers.Count;
        SwitchToPlayer(nextIndex);
    }

    public void SwitchToPreviousPlayer() // 切换到上个角色
    {
        if (PlayerControllers.Count == 0) return;
        var prevIndex = (_currentPlayerIndex - 1 + PlayerControllers.Count) % PlayerControllers.Count;
        SwitchToPlayer(prevIndex);
    }

    public void PlayerEnter(int playerindex)
    {
        /* 方案 A：激活-休眠（快，不销毁） */
        if (playerindex < PlayerInstances.Count && PlayerInstances[playerindex] != null)
        {
            PlayerInstances[playerindex].SetActive(true);
            PlayerControllers[playerindex].StateMachine.ChangeState<SwitchInState>();
        }
    }

    public void PlayerExit()
    {
        if (_currentPlayerIndex < 0) return; // 如果当前角色索引小于0，则返回
        if (_currentPlayerIndex < PlayerControllers.Count && PlayerControllers[_currentPlayerIndex] != null)
            PlayerControllers[_currentPlayerIndex].StateMachine.ChangeState<SwitchOutState>();
        //_playerInstances[CurrentPlayerIndex].SetActive(false);
    }
}