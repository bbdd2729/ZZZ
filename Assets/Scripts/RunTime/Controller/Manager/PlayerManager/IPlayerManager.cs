using System;
using System.Collections.Generic;

public interface IPlayerManager
{
    PlayerController CurrentPlayer { get; }
    IReadOnlyList<PlayerController> PlayerControllers { get; }
    
    void Initialize();
    void SwitchNextPlayer();
    void SwitchToPlayer(int playerIndex);
    void AddPlayer(PlayerController playerController);
    
    bool CanSwitchPlayer();
    event Action<PlayerController> OnPlayerSwitched;
}