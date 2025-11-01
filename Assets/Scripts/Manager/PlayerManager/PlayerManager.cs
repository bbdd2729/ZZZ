
using System.Collections.Generic;

public enum PlayerName
    {
        Anbi,
        Coein,
        Nike,
    }


public class PlayerManager : SingletonBase<PlayerManager>
{
    private int                    _currentPlayerIndex = 0;
    public  List<PlayerController> PlayerControllers   = new List<PlayerController>();
    public  PlayerController       CurrentPlayer;

    public void Init()
    {
        if (PlayerControllers == null)
            PlayerControllers = new List<PlayerController>();

        PlayerControllers.Clear();
        CurrentPlayer = null;
        _currentPlayerIndex = 0;

        DebugX.Instance.Log("PlayerManager 初始化完成");
    }

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

    public void SwitchToPlayer(int playerIndex)
    {
        _currentPlayerIndex = playerIndex;
        CurrentPlayer = PlayerControllers[_currentPlayerIndex];
        CurrentPlayer._stateMachine.ChangeState<SwitchInState>();
    }


    public void LoadPlayer()
    {
        foreach (var playerController in PlayerControllers)
        {
            playerController._stateMachine.ChangeState<IdleState>();
        }
    }
}