public class RunState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine._animator.Play("Run");
        StateMachine._playerController.InputSystem.OnMoveCanceled += OnMoveCanceled;
        StateMachine._playerController.InputSystem.OnEvadeEvent += OnEvadeEvent;
        StateMachine._playerController.InputSystem.OnBigSkillEvent += OnBigSkill;
        StateMachine._playerController.InputSystem.OnAttackEvent += OnAttack;
    }

    public override void Update()
    {
        StateMachine._playerController.SetCharacterRotation();
    }

    public override void OnExit()
    {
        StateMachine._playerController.InputSystem.OnMoveCanceled -= OnMoveCanceled;
        StateMachine._playerController.InputSystem.OnEvadeEvent -= OnEvadeEvent;
        StateMachine._playerController.InputSystem.OnBigSkillEvent -= OnBigSkill;
        StateMachine._playerController.InputSystem.OnAttackEvent -= OnAttack;
        base.OnExit();
    }
}