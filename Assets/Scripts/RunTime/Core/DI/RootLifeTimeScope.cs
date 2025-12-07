using VContainer;
using VContainer.Unity;

public class RootLifeTimeScope : LifetimeScope
{
    
    public       GameConfig      Config;
    public static IObjectResolver RootContainer { get; private set; } = null; 
    
    protected override void Configure(IContainerBuilder builder)
    {
        
    }
    
    protected override void Awake()
    {
        
    }
}
