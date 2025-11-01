using System.Collections.Generic;
using Sirenix.OdinInspector;

public enum PlayerName
    {
        Anbi,
        Coein,
        Nike,
    }


public class PlayerManager : SingletonBase<PlayerManager>
{
    [ShowInInspector] private int                    _currentPlayerIndex = 0;
    [ShowInInspector] public  List<PlayerController> PlayerControllers   = new List<PlayerController>();
    public                    PlayerController       CurrentPlayer;

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
        if (CurrentPlayer._stateMachine.StateLocked) return;
        CurrentPlayer._stateMachine.ChangeState<SwitchOutState>();
        _currentPlayerIndex = (_currentPlayerIndex + 1) % PlayerControllers.Count;
        CurrentPlayer = PlayerControllers[_currentPlayerIndex];
        CurrentPlayer.enabled = true;
        CurrentPlayer._stateMachine.ChangeState<SwitchInState>();
    }

    public void SwitchToPlayer(int playerIndex)
    {
        if (CurrentPlayer._stateMachine.StateLocked) return;
        CurrentPlayer._stateMachine.ChangeState<SwitchOutState>();
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