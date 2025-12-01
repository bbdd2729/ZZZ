public interface IStateMachineFactory
{
    IStateMachine CreateStateMachine(PlayerController playerController);
}