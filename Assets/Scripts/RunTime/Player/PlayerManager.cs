using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerName
{
    Anbi,
    Coein,
    Nike,
}

/*public class PlayerManager : SingletonBase<PlayerManager>, IPlayerManager
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
}*/



public class PlayerManager : MonoBehaviour, IPlayerManager
{
    #region 字段定义

    public           PlayerController CurrentPlayer     { get; }

    [Header("角色预制体列表——按顺序拖进来")] [SerializeField]
    private List<GameObject> _playerPrefabs;                    // 角色预制体列表
    private List<GameObject>              _playerInstances = new List<GameObject>(); // 角色对象实例列表
    public  List<PlayerController>        PlayerControllers { get; private set; } = new List<PlayerController>();
    public event Action<PlayerController> OnPlayerSwitched;

    public bool SwitchIsLocked { get; private set; }

    private int _currentPlayerIndex;
    
    

    public int CurrentPlayerIndex
    {
        get => _currentPlayerIndex;
    }

    #endregion

    #region 初始化

    /// <summary>
    /// PlayerManager构造函数
    /// </summary>
    /// <param name="characterPrefabs">角色预制体列表</param>
    /// <param name="transform">角色生成点</param>
    public PlayerManager(List<GameObject> characterPrefabs, Transform transform)
    {
        Initialize(characterPrefabs, transform);
    }
    public void Initialize(List<GameObject> characterPrefabs, Transform _transform)
    {
        
        
        if (characterPrefabs == null || characterPrefabs.Count == 0)
        {
            Debug.LogError("[PlayerManager] 角色预制体列表为空！");
            return;
        }

        _playerPrefabs = characterPrefabs; // 保存角色预制体列表

        for (int i = 0; i < _playerPrefabs.Count; i++)
        {
            
            GameObject go = Instantiate(characterPrefabs[i], _transform.position, Quaternion.identity); // 创建角色对象实例
            _playerInstances.Add(go);                                                                   // 保存角色对象实例
            RootLifeTimeScope.RootContainer.Inject(go);
            Debug.Log("[PlayerManager] 创建角色对象实例：" + go.name);
            go.SetActive(false);                                                                        // 隐藏角色
            var ctrl = go.GetComponent<PlayerController>();                                             // 获取角色控制器
            if (ctrl != null) PlayerControllers.Add(ctrl);                                              // 添加到控制器列表
        }
    } // 初始化
    

    #endregion
    
    #region 角色切换逻辑

    public void SwitchToPlayer(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= _playerPrefabs.Count)
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
    }

    public void PlayerEnter(int playerindex)
    {
        /* 方案 A：激活-休眠（快，不销毁） */
        if (playerindex < _playerInstances.Count && _playerInstances[playerindex] != null)
        {
            _playerInstances[playerindex].SetActive(true);
            PlayerControllers[playerindex].StateMachine.ChangeState<SwitchInState>();
        }
    }

    public void PlayerExit()
    {
        if (CurrentPlayerIndex < 0) return;
        /* 方案 A：只隐藏 */
        if (CurrentPlayerIndex < _playerInstances.Count && _playerInstances[CurrentPlayerIndex] != null)
            PlayerControllers[CurrentPlayerIndex].StateMachine.ChangeState<SwitchOutState>();
        //_playerInstances[CurrentPlayerIndex].SetActive(false);
    }
    /// <summary>
    /// 切换到下一个角色（循环）
    /// </summary>
    public void SwitchNextPlayer()
    {
        if (_playerPrefabs.Count == 0) return;
        int nextIndex = (_currentPlayerIndex + 1) % _playerPrefabs.Count;
        SwitchToPlayer(nextIndex);
    }
    /// <summary>
    /// 切换到上一个角色（循环）
    /// </summary>
    public void SwitchPreviousPlayer()
    {
        if (_playerPrefabs.Count == 0) return;
        int prevIndex = (_currentPlayerIndex - 1 + _playerPrefabs.Count) % _playerPrefabs.Count;
        SwitchToPlayer(prevIndex);
    }

    #endregion
    
    #region Unity生命周期函数

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    #endregion
    
    public void AddPlayer(PlayerController playerController) { }
}