

public class EvadeBackEndState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        DebugX.Instance.Log($"EvadeBackEndState OnEnter");
        StateMachine._animator.Play("Evade_Back_End");
        UniTaskTimer.StartTimer(UniTaskTimer.Mode.Once, 1.3f,
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
        DebugX.Instance.Log($"EvadeBackEndState OnExit");

    }
}