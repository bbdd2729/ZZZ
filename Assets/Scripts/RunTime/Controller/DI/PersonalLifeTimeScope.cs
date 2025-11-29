
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;


public class PersonalLifeTimeScope : LifetimeScope
{


    public       GameConfig      Config;
    public static IObjectResolver RootContainer { get; private set; } = null; 
    
    protected override void Configure(IContainerBuilder builder)
    {

        builder.RegisterInstance(Config);
        //builder.Register<GameConfig>(Lifetime.Singleton);
        
        builder.Register<SceneLoader>(Lifetime.Singleton);
        builder.Register<IntroPlayer>(Lifetime.Singleton);
        
        
        builder.RegisterEntryPoint<GameRoot>();
        
        
    }
    
    protected override void Awake()
    {
        base.Awake();
        RootContainer = Container; 
        DontDestroyOnLoad(gameObject);
    }
}
