public class EvadeState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine._animator.Play("Dash");
    }

    public override void Update() { }

    public override void OnExit()
    {
        base.OnExit();
    }
}