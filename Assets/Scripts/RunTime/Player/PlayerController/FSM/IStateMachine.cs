public interface IStateMachine
{
    BaseState CurrentState { get; }
    bool      StateLocked  { get; }

    void RegisterState<T>(T state) where T : IState;
    void ChangeState<T>() where T : IState;
    void Update();

    void Lock();
    void Unlock();
}