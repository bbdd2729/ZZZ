public class RunEndState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine._animator.CrossFade("Run_End_L", StateMachine._playerController.AnimationTranslationTime);
    }

    public override void Update()
    {
        base.Update();
        if (IsAnimationEnd()) StateMachine.ChangeState<IdleState>();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}