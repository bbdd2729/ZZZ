public class BigSkillState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine._animator.Play("BigSkill");
        UniTaskTimer.StartTimer(UniTaskTimer.Mode.Once, 0.4f,
                                UniTaskTimer.TimeSource.Scaled,
                                () => {
                                    StateMachine.StateLocked = false;
                                    StateMachine.ChangeState<IdleState>();
                                }
                               );
    }

    public override void Update() { }

    public override void OnExit()
    {
        base.OnExit();
    }
}