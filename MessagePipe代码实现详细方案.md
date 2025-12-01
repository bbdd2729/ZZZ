# MessagePipe代码实现详细方案

## 1. 依赖包配置

### 需要添加的NuGet包
```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <!-- 现有包 -->
  <package id="MemoryPack" version="1.21.4" manuallyInstalled="true" />
  <package id="MemoryPack.Core" version="1.21.4" />
  <package id="MemoryPack.Generator" version="1.21.4" />
  <package id="Microsoft.Bcl.AsyncInterfaces" version="6.0.0" />
  <package id="Microsoft.Bcl.TimeProvider" version="8.0.0" />
  <package id="R3" version="1.3.0" manuallyInstalled="true" />
  <package id="System.Collections.Immutable" version="6.0.0" />
  <package id="System.ComponentModel.Annotations" version="5.0.0" />
  <package id="System.Threading.Channels" version="8.0.0" />
  <package id="ZLinq" version="1.5.4" manuallyInstalled="true" />
  
  <!-- MessagePipe相关包 -->
  <package id="MessagePipe" version="1.8.1" manuallyInstalled="true" />
  <package id="MessagePipe.Interprocess" version="1.3.1" />
  <package id="MessagePack" version="2.5.192" />
  <package id="MessagePipe.Analyzer" version="1.8.1" />
</packages>
```

## 2. MessagePipe事件总线适配器实现

### MessagePipeEventBusAdapter.cs
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using MessagePipe;
using VContainer;

/// <summary>
/// MessagePipe事件总线适配器 - 将MessagePipe集成到现有的IEventBus接口
/// 提供高性能、零分配的事件发布/订阅功能
/// </summary>
public class MessagePipeEventBusAdapter : IEventBus, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAsyncPublisher<IEventBusEvent> _asyncPublisher;
    private readonly IAsyncSubscriber<IEventBusEvent> _asyncSubscriber;
    private readonly Dictionary<Type, IDisposable> _subscriptions = new();
    private readonly object _lock = new object();
    private bool _disposed = false;

    public MessagePipeEventBusAdapter(
        IServiceProvider serviceProvider,
        IAsyncPublisher<IEventBusEvent> asyncPublisher,
        IAsyncSubscriber<IEventBusEvent> asyncSubscriber)
    {
        _serviceProvider = serviceProvider;
        _asyncPublisher = asyncPublisher;
        _asyncSubscriber = asyncSubscriber;
    }

    /// <summary>
    /// 发布事件 - 同步版本
    /// </summary>
    public void Publish<T>(T eventData)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MessagePipeEventBusAdapter));
        
        // 将事件包装为IEventBusEvent以便通过MessagePipe传输
        var wrappedEvent = new EventBusEventWrapper<T>(eventData);
        
        // 使用异步发布但立即等待完成
        _asyncPublisher.PublishAsync(wrappedEvent, CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 发布事件 - 异步版本
    /// </summary>
    public async Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MessagePipeEventBusAdapter));
        
        var wrappedEvent = new EventBusEventWrapper<T>(eventData);
        await _asyncPublisher.PublishAsync(wrappedEvent, cancellationToken);
    }

    /// <summary>
    /// 订阅事件 - 同步处理
    /// </summary>
    public void Subscribe<T>(Action<T> listener)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MessagePipeEventBusAdapter));
        
        lock (_lock)
        {
            var eventType = typeof(T);
            
            // 创建过滤器和处理器
            var subscription = _asyncSubscriber.Subscribe(new EventHandler<T>(listener), 
                x => x is EventBusEventWrapper<T>);
            
            _subscriptions[eventType] = subscription;
        }
    }

    /// <summary>
    /// 订阅事件 - 异步处理
    /// </summary>
    public void SubscribeAsync<T>(Func<T, Task> asyncListener)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MessagePipeEventBusAdapter));
        
        lock (_lock)
        {
            var eventType = typeof(T);
            
            var subscription = _asyncSubscriber.Subscribe(new AsyncEventHandler<T>(asyncListener), 
                x => x is EventBusEventWrapper<T>);
            
            _subscriptions[eventType] = subscription;
        }
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    public void Unsubscribe<T>(Action<T> listener)
    {
        if (_disposed) return;
        
        lock (_lock)
        {
            var eventType = typeof(T);
            
            if (_subscriptions.TryGetValue(eventType, out var subscription))
            {
                subscription.Dispose();
                _subscriptions.Remove(eventType);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        lock (_lock)
        {
            foreach (var subscription in _subscriptions.Values)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
        }
        
        _disposed = true;
    }
}

/// <summary>
/// 事件总线事件接口 - 用于MessagePipe消息包装
/// </summary>
public interface IEventBusEvent
{
    Type GetEventType();
    object GetEventData();
}

/// <summary>
/// 事件包装器 - 用于通过MessagePipe传输具体事件
/// </summary>
public class EventBusEventWrapper<T> : IEventBusEvent
{
    public T EventData { get; }

    public EventBusEventWrapper(T eventData)
    {
        EventData = eventData;
    }

    public Type GetEventType() => typeof(T);
    public object GetEventData() => EventData;
}

/// <summary>
/// 同步事件处理器
/// </summary>
public class EventHandler<T> : IAsyncMessageHandler<IEventBusEvent>
{
    private readonly Action<T> _handler;

    public EventHandler(Action<T> handler)
    {
        _handler = handler;
    }

    public Task HandleAsync(IEventBusEvent message, CancellationToken cancellationToken = default)
    {
        if (message is EventBusEventWrapper<T> wrapper)
        {
            _handler(wrapper.EventData);
        }
        return Task.CompletedTask;
    }
}

/// <summary>
/// 异步事件处理器
/// </summary>
public class AsyncEventHandler<T> : IAsyncMessageHandler<IEventBusEvent>
{
    private readonly Func<T, Task> _handler;

    public AsyncEventHandler(Func<T, Task> handler)
    {
        _handler = handler;
    }

    public async Task HandleAsync(IEventBusEvent message, CancellationToken cancellationToken = default)
    {
        if (message is EventBusEventWrapper<T> wrapper)
        {
            await _handler(wrapper.EventData);
        }
    }
}
```

## 3. MessagePipe配置类

### MessagePipeConfiguration.cs
```csharp
using MessagePipe;
using VContainer;
using VContainer.Unity;

/// <summary>
/// MessagePipe配置类 - 配置MessagePipe服务和依赖注入
/// </summary>
public static class MessagePipeConfiguration
{
    /// <summary>
    /// 添加MessagePipe事件系统到依赖注入容器
    /// </summary>
    public static void AddMessagePipeEventSystem(IContainerBuilder builder)
    {
        // 配置MessagePipe选项
        var options = builder.RegisterMessagePipe(options =>
        {
            // 启用异步处理
            options.EnableAsyncProcessing = true;
            
            // 配置内存池大小
            options.InstanceLifetime = InstanceLifetime.Singleton;
            
            // 启用消息过滤
            options.EnableMessageFiltering = true;
            
            // 配置异步处理器数量
            options.AsyncProcessingNum = Environment.ProcessorCount;
        });

        // 注册事件总线事件类型
        builder.RegisterMessageHandlerFilter<EventHandlerFilter>();
        
        // 注册异步发布者和订阅者
        builder.RegisterAsyncMessageHandler<IEventBusEvent, IAsyncMessageHandler<IEventBusEvent>>(
            EventHandlerFactory, 
            options);
        
        // 注册MessagePipeEventBusAdapter作为IEventBus的实现
        builder.Register<MessagePipeEventBusAdapter>(Lifetime.Singleton)
               .As<IEventBus>()
               .AsSelf();
    }

    /// <summary>
    /// 事件处理器工厂
    /// </summary>
    private static IAsyncMessageHandler<IEventBusEvent> EventHandlerFactory(IObjectResolver resolver)
    {
        return resolver.Resolve<IAsyncSubscriber<IEventBusEvent>>()
                      .Subscribe(new EventHandlerFilter());
    }
}

/// <summary>
/// 事件处理器过滤器 - 用于消息过滤和预处理
/// </summary>
public class EventHandlerFilter : IAsyncMessageHandlerFilter<IEventBusEvent>
{
    public async ValueTask HandleAsync(
        IEventBusEvent message, 
        CancellationToken cancellationToken, 
        Func<IEventBusEvent, CancellationToken, ValueTask> next)
    {
        // 可以在这里添加日志、性能监控、消息验证等功能
        try
        {
            await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // 异常处理
            UnityEngine.Debug.LogError($"MessagePipe事件处理错误: {ex.Message}");
            throw;
        }
    }
}
```

## 4. 事件定义迁移方案

### 现有事件定义
```csharp
// 原定义
[Serializable]
public class PlayerSwitchedEvent
{
    public PlayerController PreviousPlayer { get; set; }
    public PlayerController CurrentPlayer { get; set; }
    public float SwitchStartTime { get; set; }
    public float SwitchEndTime { get; set; }
    public float SwitchDuration => SwitchEndTime - SwitchStartTime;
}
```

### MessagePipe兼容定义
```csharp
using MessagePack;

[MessagePackObject]
[Serializable]
public class PlayerSwitchedEvent
{
    [Key(0)]
    public PlayerController PreviousPlayer { get; set; }
    
    [Key(1)]
    public PlayerController CurrentPlayer { get; set; }
    
    [Key(2)]
    public float SwitchStartTime { get; set; }
    
    [Key(3)]
    public float SwitchEndTime { get; set; }
    
    [IgnoreMember]
    public float SwitchDuration => SwitchEndTime - SwitchStartTime;
}
```

## 5. 依赖注入配置更新

### PersonalLifeTimeScope.cs更新
```csharp
public class PersonalLifeTimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // 现有配置...
        
        // 添加MessagePipe事件系统
        MessagePipeConfiguration.AddMessagePipeEventSystem(builder);
        
        // 可以保留旧的EventBusAdapter作为备选
        // builder.Register<EventBusAdapter>(Lifetime.Singleton).As<IEventBus>();
    }
}
```

## 6. 使用示例

### 事件发布
```csharp
public class PlayerSwitchManager
{
    private readonly IEventBus _eventBus;
    
    public async Task CompleteSwitchAsync()
    {
        // 同步发布
        _eventBus.Publish(new PlayerSwitchCompletedEvent(fromPlayer, toPlayer));
        
        // 异步发布
        await _eventBus.PublishAsync(new PlayerSwitchCompletedEvent(fromPlayer, toPlayer));
    }
}
```

### 事件订阅
```csharp
public class PlayerManager
{
    private readonly IEventBus _eventBus;
    
    public void Initialize()
    {
        // 同步订阅
        _eventBus.Subscribe<PlayerSwitchCompletedEvent>(OnPlayerSwitchCompleted);
        
        // 异步订阅
        _eventBus.SubscribeAsync<PlayerSwitchCompletedEvent>(OnPlayerSwitchCompletedAsync);
    }
    
    private void OnPlayerSwitchCompleted(PlayerSwitchCompletedEvent evt)
    {
        // 处理事件
    }
    
    private async Task OnPlayerSwitchCompletedAsync(PlayerSwitchCompletedEvent evt)
    {
        // 异步处理事件
        await Task.Delay(100);
    }
}
```

## 7. 性能监控和调试

### 性能监控器
```csharp
public class MessagePipePerformanceMonitor
{
    private readonly Dictionary<Type, EventMetrics> _metrics = new();
    
    public void RecordEventPublish<T>(TimeSpan duration)
    {
        var type = typeof(T);
        if (!_metrics.TryGetValue(type, out var metrics))
        {
            metrics = new EventMetrics();
            _metrics[type] = metrics;
        }
        
        metrics.PublishCount++;
        metrics.TotalPublishTime += duration;
    }
    
    public class EventMetrics
    {
        public long PublishCount { get; set; }
        public TimeSpan TotalPublishTime { get; set; }
        public TimeSpan AveragePublishTime => PublishCount > 0 ? 
            TimeSpan.FromTicks(TotalPublishTime.Ticks / PublishCount) : TimeSpan.Zero;
    }
}
```

## 8. 测试策略

### 单元测试
- 事件发布/订阅功能测试
- 异步处理测试
- 异常处理测试
- 性能基准测试

### 集成测试
- 与现有系统兼容性测试
- 依赖注入测试
- 多线程安全测试

### 性能测试
- 吞吐量测试
- 内存分配测试
- 延迟测试