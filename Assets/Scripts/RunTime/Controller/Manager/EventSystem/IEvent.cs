public interface IEvent
{
    public void Subscribe<T>();
    public void Unsubscribe<T>();
    public void Trigger<T>();
}