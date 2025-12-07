using VContainer;
using VContainer.Unity;

public class RootLifeTimeScope : LifetimeScope
{
    
    public       GameConfig      Config;
    public static IObjectResolver RootContainer { get; private set; }
    
    protected override void Configure(IContainerBuilder builder)
    {
        // 注册配置
        builder.RegisterInstance(Config);
        // 注册基础服务
        builder.Register<SceneLoader>(Lifetime.Singleton).As<ISceneLoader>();
        builder.Register<IntroPlayer>(Lifetime.Singleton);
        
        //builder.Register<CameraSystem>(Lifetime.Singleton).As<ICameraSystem>();
        
        // 注册事件系统适配器
        builder.Register<EventBusAdapter>(Lifetime.Singleton).As<IEventBus>();
        
        // 注册新的架构服务
        //builder.Register<PlayerManagerService>(Lifetime.Singleton).As<IPlayerManager>();
        builder.Register<StateMachineFactory>(Lifetime.Singleton).As<IStateMachineFactory>();
        builder.Register<PlayerSwitchManager>(Lifetime.Singleton).As<IPlayerSwitchManager>();
        builder.Register<PlayerObjectPool>(Lifetime.Singleton).As<IPlayerObjectPool>();
        
        // 保持向后兼容 - 注册PlayerManager单例
        
        // 注册入口点
        builder.RegisterComponentInHierarchy<GameMain>();
        //builder.Register<InputSystem>(Lifetime.Singleton).AsSelf().As<IInputSystem>();
        builder.RegisterComponentInHierarchy<PlayerManager>().AsSelf().As<IPlayerManager>();
        
        
        
        
            
    }
    
    
    
    
    protected override void Awake()
    {
        base.Awake();
        if (RootContainer == null) RootContainer = Container; // 只赋一次
        DontDestroyOnLoad(gameObject);
        
        
    }
}
