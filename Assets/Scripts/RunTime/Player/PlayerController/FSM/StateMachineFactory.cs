using VContainer;

public class StateMachineFactory : IStateMachineFactory
{
    private readonly IObjectResolver _container;

    [Inject]
    public StateMachineFactory(IObjectResolver container)
    {
        _container = container;
    }

    public IStateMachine CreateStateMachine(PlayerController playerController)
    {
        // 创建状态机实例
        var stateMachine = new StateMachine(playerController);

        // 注册所有状态
        RegisterPlayerStates(stateMachine, playerController);

        return stateMachine;
    }

    private void RegisterPlayerStates(IStateMachine stateMachine, PlayerController playerController)
    {
        // 创建状态实例
        var idleState = new IdleState();
        var walkState = new WalkState();
        var runState = new RunState();
        var attackState = new AttackState();
        var attackEndState = new AttackEndState();
        var evadeState = new EvadeState();
        var evadeBackState = new EvadeBackState();
        var evadeBackEndState = new EvadeBackEndState();
        var bigSkillState = new BigSkillState();
        var switchInState = new SwitchInState();
        var switchOutState = new SwitchOutState();

        // 注册状态
        stateMachine.RegisterState(idleState);
        stateMachine.RegisterState(walkState);
        stateMachine.RegisterState(runState);
        stateMachine.RegisterState(attackState);
        stateMachine.RegisterState(attackEndState);
        stateMachine.RegisterState(evadeState);
        stateMachine.RegisterState(evadeBackState);
        stateMachine.RegisterState(evadeBackEndState);
        stateMachine.RegisterState(bigSkillState);
        stateMachine.RegisterState(switchInState);
        stateMachine.RegisterState(switchOutState);

        // 设置初始状态
        stateMachine.ChangeState<IdleState>();
    }
}