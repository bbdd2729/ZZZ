public class EvadeEndState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        DebugX.Instance.Log("EvadeEndState OnEnter");
        StateMachine._animator.CrossFade("Evade_End", StateMachine._playerController.AnimationTranslationTime);
    }

    public override void Update()
    {
        base.Update();
        if (IsAnimationEnd())
        {
            StateMachine.StateLocked = false;
            StateMachine.ChangeState<IdleState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}