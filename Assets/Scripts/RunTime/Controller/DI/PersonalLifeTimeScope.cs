using VContainer;
using VContainer.Unity;

public class PersonalLifeTimeScope : LifetimeScope
{


    public       GameConfig      Config;
    public static IObjectResolver RootContainer { get; private set; } = null; 
    
    protected override void Configure(IContainerBuilder builder)
    {
        // 注册配置
        builder.RegisterInstance(Config);
        
        // 注册基础服务
        builder.Register<SceneLoader>(Lifetime.Singleton);
        builder.Register<IntroPlayer>(Lifetime.Singleton);
        
        // 注册事件系统适配器
        builder.Register<EventBusAdapter>(Lifetime.Singleton).As<IEventBus>();
        
        // 注册新的架构服务
        builder.Register<PlayerManagerService>(Lifetime.Singleton).As<IPlayerManager>();
        builder.Register<StateMachineFactory>(Lifetime.Singleton).As<IStateMachineFactory>();
        builder.Register<PlayerSwitchManager>(Lifetime.Singleton).As<IPlayerSwitchManager>();
        builder.Register<PlayerObjectPool>(Lifetime.Singleton).As<IPlayerObjectPool>();
        
        // 保持向后兼容 - 注册PlayerManager单例
        builder.Register<PlayerManager>(Lifetime.Singleton);
        
        // 注册入口点
        builder.RegisterEntryPoint<GameRoot>();
    }
    
    protected override void Awake()
    {
        base.Awake();
        RootContainer = Container;
        DontDestroyOnLoad(gameObject);
        
        // 确保单例实例可用并保持向后兼容
        var playerManager = Container.Resolve<PlayerManager>();
        if (playerManager != null)
        {
            PlayerManager.Instance.Init();
        }
    }
}
