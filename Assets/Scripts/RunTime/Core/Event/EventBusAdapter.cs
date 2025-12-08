using System;

/// <summary>
///     EventBus适配器 - 将现有的EventBus单例适配到IEventBus接口
///     用于依赖注入兼容性
/// </summary>
public class EventBusAdapter : IEventBus
{
    public void Publish<T>(T eventData)
    {
        EventBus.Instance.GetGameEvent<T>().Publish(eventData);
    }

    public void Subscribe<T>(Action<T> listener)
    {
        EventBus.Instance.GetGameEvent<T>().Subscribe(listener);
    }

    public void Unsubscribe<T>(Action<T> listener)
    {
        EventBus.Instance.GetGameEvent<T>().Unsubscribe(listener);
    }
}