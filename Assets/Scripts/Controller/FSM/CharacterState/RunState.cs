public class RunState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine._animator.Play("Run");
        InputSystem.Instance.OnMoveCanceled += OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent += OnEvadeEvent;
    }

    public override void Update()
    {
        PlayerController.SetCharacterRotation();
    }

    public override void OnExit()
    {
        InputSystem.Instance.OnMoveCanceled -= OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeEvent;
        base.OnExit();
    }
}