public class EvadeBackState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        StateMachine._animator.Play("Evade_Back");
        UniTaskTimer.StartTimer(UniTaskTimer.Mode.Once, 0.4f,
                                UniTaskTimer.TimeSource.Scaled,
                                () => {
                                    StateMachine.StateLocked = false;
                                    StateMachine.ChangeState<IdleState>();
                                }
                                );
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}