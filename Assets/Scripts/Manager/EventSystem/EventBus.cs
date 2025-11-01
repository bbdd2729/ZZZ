using System;
using System.Collections.Generic;

public  class EventBus : SingletonBase<EventBus>
{
    private readonly Dictionary<Type, object> _events = new();

    // 获取事件实例
    public GameEvent<T> GetGameEvent<T>()
    {
        var type = typeof(T);
        if (!_events.ContainsKey(type)) _events[type] = new GameEvent<T>();

        return (GameEvent<T>) _events[type];
    }
}