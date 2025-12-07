using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerManager
{
    PlayerController               CurrentPlayer      { get; }
    List<PlayerController>         PlayerControllers  { get; }
    bool                           SwitchIsLocked     { get; }
    int                            CurrentPlayerIndex { get; }
    event Action<PlayerController> OnPlayerSwitched;
    void                           Initialize(List<GameObject> characterPrefabs, Transform _transform);
    void                           SwitchToPlayer(int playerIndex);
    void                           PlayerEnter(int index);
    void           PlayerExit();

    /// <summary>
    /// 切换到下一个角色（循环）
    /// </summary>
    void SwitchNextPlayer();

    /// <summary>
    /// 切换到上一个角色（循环）
    /// </summary>
    void SwitchPreviousPlayer();

    void AddPlayer(PlayerController playerController);
}