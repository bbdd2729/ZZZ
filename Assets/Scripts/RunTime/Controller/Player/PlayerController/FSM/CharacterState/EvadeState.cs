public class EvadeState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        StateMachine._animator.Play("Evade_Front");
        UniTaskTimer.StartTimer(UniTaskTimer.Mode.Once,
                                0.4f, UniTaskTimer.TimeSource.Scaled,
                                () => {
                                    StateMachine.StateLocked = false;
                                    StateMachine.ChangeState<RunState>();
                                }
                                );
    }

    public override void Update() { }

    public override void OnExit()
    {
        base.OnExit();
    }
}