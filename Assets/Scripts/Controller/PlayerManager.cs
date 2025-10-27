
using System.Collections.Generic;

public enum PlayerName
    {
        Anbi,
        Player2,

    }


public class PlayerManager : SingletonBase<PlayerManager>
{
    private int _currentPlayerIndex = 0;
    public List<PlayerController> PlayerControllers;

    public void AddPlayer(PlayerController playerController)
    {
        PlayerControllers.Add(playerController);
    }

    public void SwitchNextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % PlayerControllers.Count;
        CurrentPlayer = PlayerControllers[_currentPlayerIndex];
        CurrentPlayer._stateMachine.ChangeState<SwitchInState>();
    }
}