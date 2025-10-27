

public class SwitchInState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        DebugX.Instance.Log($"SwitchInState OnEnter");
        StateMachine._animator.Play("Switch_In");
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
        DebugX.Instance.Log($"SwitchInState OnExit");
    }
}