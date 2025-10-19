public class BigSkillState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine._animator.Play("BigSkill");
    }

    public override void Update() { }

    public override void OnExit()
    {
        StateMachine.ChangeState<IdleState>();
        base.OnExit();
    }
}