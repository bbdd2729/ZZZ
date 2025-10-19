using System;

public class GameEvent<T>
{
    private event Action<T> OnEvent;

    public void Publish(T data)
    {
        OnEvent?.Invoke(data);
    }

    public void Subscribe(Action<T> listener)
    {
        OnEvent += listener;
    }

    public void Unsubscribe(Action<T> listener)
    {
        OnEvent -= listener;
    }
}