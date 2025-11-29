# VContainer重构方案

## 项目现状分析

### 当前架构问题
1. **混合架构**：项目同时使用VContainer和单例模式，造成架构不一致
2. **紧耦合**：管理器之间直接依赖具体实现，而非抽象接口
3. **生命周期管理混乱**：各管理器自行管理生命周期，缺乏统一控制
4. **测试困难**：单例模式导致单元测试难以进行依赖注入

### 需要重构的单例管理器
- PlayerManager - 玩家管理器
- InputSystem - 输入系统
- EventBus - 事件总线
- CameraSystem - 相机系统
- UpdateManager - 更新管理器
- DebugX - 调试工具

## 重构架构设计

### 1. 分层架构

```
Presentation Layer (表示层)
├── GameMain.cs (游戏入口)
├── GameRoot.cs (VContainer入口)
└── LifetimeScopes (作用域配置)

Application Layer (应用层)
├── IPlayerManager / PlayerManager
├── IInputSystem / InputSystem
├── IEventBus / EventBus
├── ICameraSystem / CameraSystem
└── IUpdateManager / UpdateManager

Domain Layer (领域层)
├── PlayerController
├── StateMachine
└── GameEvents

Infrastructure Layer (基础设施层)
├── SceneLoader
├── IntroPlayer
└── GameConfig
```

### 2. 服务接口设计

#### IPlayerManager
```csharp
public interface IPlayerManager
{
    IReadOnlyList<PlayerController> PlayerControllers { get; }
    PlayerController CurrentPlayer { get; }
    void AddPlayer(PlayerController controller);
    void SwitchNextPlayer();
    void SwitchToPlayer(int index);
    void LoadPlayer();
}
```

#### IInputSystem
```csharp
public interface IInputSystem
{
    InputSystem_Actions InputActions { get; }
    Vector2 MoveDirectionInput { get; }
    Vector2 CameraLook { get; }
    bool Run { get; }
    bool Walk { get; }
    bool Space { get; }
    
    event Action<InputAction.CallbackContext> OnMovePerformed;
    event Action<InputAction.CallbackContext> OnMoveCanceled;
    event Action<InputAction.CallbackContext> OnEvadeEvent;
    event Action<InputAction.CallbackContext> OnWalkEvent;
    event Action<InputAction.CallbackContext> SwitchCharacterEvent;
    event Action<InputAction.CallbackContext> OnBigSkillEvent;
    event Action<InputAction.CallbackContext> OnAttackEvent;
}
```

#### IEventBus
```csharp
public interface IEventBus
{
    GameEvent<T> GetGameEvent<T>();
    void Publish<T>(T eventData);
    IDisposable Subscribe<T>(Action<T> callback);
}
```

#### ICameraSystem
```csharp
public interface ICameraSystem
{
    Vector3 CamPosition { get; }
    Quaternion CamRotation { get; }
    Transform LookAtPoint { get; set; }
    void Initialize();
    void UpdateCamera();
}
```

#### IUpdateManager
```csharp
public interface IUpdateManager
{
    void RegisterUpdate(Action updateAction);
    void RegisterFixedUpdate(Action fixedUpdateAction);
    void RegisterLateUpdate(Action lateUpdateAction);
    void UnregisterUpdate(Action updateAction);
    void UnregisterFixedUpdate(Action fixedUpdateAction);
    void UnregisterLateUpdate(Action lateUpdateAction);
}
```

### 3. 生命周期作用域设计

#### GameLifetimeScope (游戏级单例)
- GameConfig
- EventBus
- UpdateManager
- SceneLoader
- IntroPlayer

#### PlayerLifetimeScope (玩家相关)
- PlayerManager
- InputSystem
- PlayerControllers (多个实例)

#### CameraLifetimeScope (相机相关)
- CameraSystem
- Cinemachine组件

### 4. 重构步骤

#### 阶段1：接口提取和基础重构
1. 创建接口定义文件
2. 重构管理器实现接口
3. 保持向后兼容性

#### 阶段2：VContainer集成
1. 创建GameLifetimeScope
2. 配置服务注册
3. 实现构造函数注入

#### 阶段3：入口点重构
1. 创建新的GameEntryPoint
2. 重构GameMain使用依赖注入
3. 实现统一的初始化流程

#### 阶段4：清理和优化
1. 移除单例基类依赖
2. 优化服务间通信
3. 添加单元测试支持

## 具体实现方案

### 1. PlayerManager重构

#### 原实现问题
```csharp
public class PlayerManager : SingletonBase<PlayerManager>
{
    // 直接依赖具体实现
    public void Init()
    {
        InputSystem.Instance.SwitchCharacterEvent += _ => SwitchNextPlayer();
    }
}
```

#### 新实现方案
```csharp
public interface IPlayerManager
{
    IReadOnlyList<PlayerController> PlayerControllers { get; }
    PlayerController CurrentPlayer { get; }
    void AddPlayer(PlayerController controller);
    void SwitchNextPlayer();
    void SwitchToPlayer(int index);
    void LoadPlayer();
}

public class PlayerManager : IPlayerManager, IDisposable
{
    private readonly IInputSystem _inputSystem;
    private readonly List<PlayerController> _playerControllers = new();
    
    [Inject]
    public PlayerManager(IInputSystem inputSystem)
    {
        _inputSystem = inputSystem;
        SetupInputEvents();
    }
    
    private void SetupInputEvents()
    {
        _inputSystem.SwitchCharacterEvent += OnSwitchCharacter;
    }
    
    public void Dispose()
    {
        _inputSystem.SwitchCharacterEvent -= OnSwitchCharacter;
    }
    
    // 实现接口方法...
}
```

### 2. InputSystem重构

#### 原实现问题
```csharp
public class InputSystem : SingletonBase<InputSystem>
{
    public InputSystem()
    {
        Debug.Log("已创建InputSystem实例");
        if (InputActions == null) InputActions = new InputSystem_Actions();
        Init();
    }
}
```

#### 新实现方案
```csharp
public interface IInputSystem
{
    InputSystem_Actions InputActions { get; }
    Vector2 MoveDirectionInput { get; }
    // 其他属性...
}

public class InputSystem : IInputSystem, IDisposable
{
    public InputSystem_Actions InputActions { get; private set; }
    
    public InputSystem()
    {
        InputActions = new InputSystem_Actions();
        SetupInputCallbacks();
    }
    
    private void SetupInputCallbacks()
    {
        InputActions.Player.Move.performed += OnMovePerformed;
        // 其他事件绑定...
    }
    
    public void Dispose()
    {
        InputActions.Player.Move.performed -= OnMovePerformed;
        // 其他事件解绑...
    }
    
    // 实现接口属性...
}
```

### 3. EventBus重构

#### 原实现问题
```csharp
public class EventBus : SingletonBase<EventBus>
{
    private readonly Dictionary<Type, object> _events = new();
}
```

#### 新实现方案
```csharp
public interface IEventBus
{
    GameEvent<T> GetGameEvent<T>();
    void Publish<T>(T eventData);
    IDisposable Subscribe<T>(Action<T> callback);
}

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, object> _events = new();
    private readonly object _lock = new object();
    
    public GameEvent<T> GetGameEvent<T>()
    {
        lock (_lock)
        {
            var type = typeof(T);
            if (!_events.ContainsKey(type))
            {
                _events[type] = new GameEvent<T>();
            }
            return (GameEvent<T>)_events[type];
        }
    }
    
    public void Publish<T>(T eventData)
    {
        GetGameEvent<T>().Publish(eventData);
    }
    
    public IDisposable Subscribe<T>(Action<T> callback)
    {
        return GetGameEvent<T>().Subscribe(callback);
    }
}
```

### 4. 统一入口点设计

#### GameEntryPoint
```csharp
public class GameEntryPoint : IStartable
{
    private readonly IPlayerManager _playerManager;
    private readonly IInputSystem _inputSystem;
    private readonly IEventBus _eventBus;
    private readonly GameConfig _gameConfig;
    private readonly ISceneLoader _sceneLoader;
    
    [Inject]
    public GameEntryPoint(
        IPlayerManager playerManager,
        IInputSystem inputSystem,
        IEventBus eventBus,
        GameConfig gameConfig,
        ISceneLoader sceneLoader)
    {
        _playerManager = playerManager;
        _inputSystem = inputSystem;
        _eventBus = eventBus;
        _gameConfig = gameConfig;
        _sceneLoader = sceneLoader;
    }
    
    public void Start()
    {
        InitializeSystems();
        LoadGameData();
        SetupEventHandlers();
    }
    
    private void InitializeSystems()
    {
        // 初始化各个系统
        _inputSystem.InputActions.Enable();
        Debug.Log("游戏系统初始化完成");
    }
    
    private void LoadGameData()
    {
        // 加载游戏配置和数据
        Debug.Log("游戏数据加载完成");
    }
    
    private void SetupEventHandlers()
    {
        // 设置事件处理
        Debug.Log("事件处理设置完成");
    }
}
```

### 5. LifetimeScope配置

#### GameLifetimeScope
```csharp
public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameConfig _gameConfig;
    
    protected override void Configure(IContainerBuilder builder)
    {
        // 注册配置
        builder.RegisterInstance(_gameConfig);
        
        // 注册基础设施服务
        builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);
        builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
        builder.Register<IUpdateManager, UpdateManager>(Lifetime.Singleton);
        
        // 注册应用服务
        builder.Register<IInputSystem, InputSystem>(Lifetime.Singleton);
        builder.Register<ICameraSystem, CameraSystem>(Lifetime.Singleton);
        
        // 注册玩家相关服务
        builder.Register<IPlayerManager, PlayerManager>(Lifetime.Singleton);
        
        // 注册入口点
        builder.RegisterEntryPoint<GameEntryPoint>();
    }
}
```

## 重构优势

### 1. 解耦和可测试性
- 通过接口抽象降低组件间耦合
- 支持依赖注入，便于单元测试
- 可以轻松替换具体实现

### 2. 生命周期管理
- 统一的生命周期管理
- 支持作用域嵌套
- 自动处理资源释放

### 3. 可扩展性
- 易于添加新的管理器
- 支持模块化开发
- 配置灵活

### 4. 性能优化
- 减少单例查找开销
- 支持延迟初始化
- 内存管理更精细

## 实施计划

### 第一阶段（1-2天）
1. 创建接口定义
2. 重构PlayerManager和InputSystem
3. 测试基本功能

### 第二阶段（2-3天）
1. 重构EventBus和CameraSystem
2. 实现GameEntryPoint
3. 配置LifetimeScope

### 第三阶段（1-2天）
1. 重构UpdateManager和DebugX
2. 优化初始化流程
3. 完整功能测试

### 第四阶段（1天）
1. 代码清理和优化
2. 文档编写
3. 性能测试

## 风险评估和缓解

### 风险1：功能回归
- **缓解**：逐步重构，保持向后兼容
- **测试**：每个阶段都进行完整功能测试

### 风险2：性能影响
- **缓解**：使用性能分析工具监控
- **优化**：必要时使用对象池和缓存

### 风险3：团队学习成本
- **缓解**：提供详细文档和培训
- **支持**：代码审查和配对编程

## 预期收益

1. **代码质量提升**：降低复杂度，提高可维护性
2. **开发效率提升**：更清晰的架构，更快的功能开发
3. **测试覆盖率提升**：更容易编写单元测试
4. **团队协作改善**：统一的架构标准
5. **性能优化**：更高效的依赖管理

## 后续优化方向

1. **模块化进一步拆分**：将系统拆分为更小的模块
2. **事件系统优化**：实现更高效的发布订阅机制
3. **状态机优化**：考虑使用更先进的状态机实现
4. **网络同步支持**：为未来网络功能做准备
5. **编辑器工具开发**：开发配置和调试工具