using System;

public interface IEventBus
{
    void Publish<T>(T eventData);
    void Subscribe<T>(Action<T> listener);
    void Unsubscribe<T>(Action<T> listener);
}