

// 可注入的角色管理服务
/*public class PlayerManagerService : IPlayerManager, IDisposable
{
    private readonly PlayerManager _playerManager;
    
    [Inject]
    public PlayerManagerService(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }
    
    public PlayerController CurrentPlayer => _playerManager.CurrentPlayer;
    public IReadOnlyList<PlayerController> PlayerControllers => _playerManager.PlayerControllers;
    
    public void Initialize() => _playerManager.Init();
    public void SwitchNextPlayer() => _playerManager.SwitchNextPlayer();
    public void SwitchToPlayer(int playerIndex) => _playerManager.SwitchToPlayer(playerIndex);
    public void AddPlayer(PlayerController playerController) => _playerManager.AddPlayer(playerController);
    
    public bool CanSwitchPlayer() => _playerManager.CanSwitchPlayer();
    
    public event Action<PlayerController> OnPlayerSwitched
    {
        add => _playerManager.OnPlayerSwitched += value;
        remove => _playerManager.OnPlayerSwitched -= value;
    }
    
    public void Dispose()
    {
        // 不能直接设置事件为null，只能通过移除所有订阅者
        // 这里我们不需要特别清理，因为PlayerManager的生命周期和Service相同
    }
}*/