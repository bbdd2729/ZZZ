public class WalkState : BaseState
{
    public override void OnEnter()
    {
        StateMachine._animator.Play("Walk");
        InputSystem.Instance.OnMoveCanceled += OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent += OnEvadeEvent;
        InputSystem.Instance.OnBigSkillEvent += OnBigSkillEvent;
    }

    public override void Update()
    {
        //if (InputSystem.Instance.PlayerMove == Vector2.zero) StateMachine.ChangeState<IdleState>();
        PlayerController.SetCharacterRotation();
    }

    public override void OnExit()
    {
        base.OnExit();
        InputSystem.Instance.OnMoveCanceled -= OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeEvent;
        InputSystem.Instance.OnBigSkillEvent -= OnBigSkillEvent;
    }
}