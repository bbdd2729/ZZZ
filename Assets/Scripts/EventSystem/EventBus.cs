using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, object> _events = new();

    // 获取事件实例
    public static GameEvent<T> GetGameEvent<T>()
    {
        var type = typeof(T);
        if (!_events.ContainsKey(type)) _events[type] = new GameEvent<T>();

        return (GameEvent<T>) _events[type];
    }
}