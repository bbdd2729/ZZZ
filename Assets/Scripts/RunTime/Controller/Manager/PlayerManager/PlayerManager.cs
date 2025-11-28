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
        if (CurrentPlayer != null && CurrentPlayer._stateMachine.StateLocked) return;
        
        // 禁用当前角色
        if (CurrentPlayer != null)
        {
            // 立即禁用输入和组件
            CurrentPlayer.SetInputActive(false);
            CurrentPlayer.enabled = false;
            CurrentPlayer._stateMachine.ChangeState<SwitchOutState>();
        }
        
        _currentPlayerIndex = (_currentPlayerIndex + 1) % PlayerControllers.Count;
        CurrentPlayer = PlayerControllers[_currentPlayerIndex];
        
        // 启用新角色，但先不启用输入，等待 SwitchInState 结束后再启用
        CurrentPlayer.gameObject.SetActive(true);
        CurrentPlayer.enabled = true; // 先启用组件，但输入控制由状态管理
        CurrentPlayer._stateMachine.ChangeState<SwitchInState>();
        
        // 在新角色切换入后，隐藏旧角色
        var oldPlayerIndex = (_currentPlayerIndex - 1 + PlayerControllers.Count) % PlayerControllers.Count;
        PlayerControllers[oldPlayerIndex].gameObject.SetActive(false);
    }

    public void SwitchToPlayer(int playerIndex)
    {
        if (CurrentPlayer != null && CurrentPlayer._stateMachine.StateLocked) return;
        
        // 禁用当前角色
        if (CurrentPlayer != null)
        {
            // 立即禁用输入和组件
            CurrentPlayer.SetInputActive(false);
            CurrentPlayer.enabled = false;
            CurrentPlayer._stateMachine.ChangeState<SwitchOutState>();
        }
        
        var oldPlayerIndex = _currentPlayerIndex;
        _currentPlayerIndex = playerIndex;
        CurrentPlayer = PlayerControllers[_currentPlayerIndex];
        
        // 启用新角色，但先不启用输入，等待 SwitchInState 结束后再启用
        CurrentPlayer.gameObject.SetActive(true);
        CurrentPlayer.enabled = true; // 先启用组件，但输入控制由状态管理
        CurrentPlayer._stateMachine.ChangeState<SwitchInState>();
        
        // 在新角色切换入后，隐藏旧角色
        if (oldPlayerIndex != _currentPlayerIndex)
        {
            PlayerControllers[oldPlayerIndex].gameObject.SetActive(false);
        }
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