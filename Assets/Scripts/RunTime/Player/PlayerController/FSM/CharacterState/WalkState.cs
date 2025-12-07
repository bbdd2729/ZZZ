public class WalkState : BaseState
{
    public override void OnEnter()
    {
        StateMachine._animator.Play("Walk");
        StateMachine._playerController.InputSystem.OnMoveCanceled += OnMoveCanceled;
        StateMachine._playerController.InputSystem.OnEvadeEvent += OnEvadeEvent;
        StateMachine._playerController.InputSystem.OnBigSkillEvent += OnBigSkill;
        StateMachine._playerController.InputSystem.OnAttackEvent += OnAttack;
        UniTaskTimer.StartTimer(UniTaskTimer.Mode.Once, 4.0f,
                                UniTaskTimer.TimeSource.Scaled,
                                () => {
                                    StateMachine.ChangeState<RunState>();
                                }
                                );
    }

    public override void Update()
    {
        //if (InputSystem.Instance.PlayerMove == Vector2.zero) StateMachine.ChangeState<IdleState>();
        StateMachine._playerController.SetCharacterRotation();
    }

    public override void OnExit()
    {
        base.OnExit();
        StateMachine._playerController.InputSystem.OnMoveCanceled -= OnMoveCanceled;
        StateMachine._playerController.InputSystem.OnEvadeEvent -= OnEvadeEvent;
        StateMachine._playerController.InputSystem.OnBigSkillEvent -= OnBigSkill;
        StateMachine._playerController.InputSystem.OnAttackEvent -= OnAttack;
    }
}