public class IdleState : BaseState
{
    public override void OnEnter()
    {
        // 播放闲置动画
        StateMachine._playerController.InputSystem.OnMovePerformed += OnMove;
        StateMachine._playerController.InputSystem.OnAttackEvent += OnAttack;
        StateMachine._playerController.InputSystem.OnEvadeEvent += OnEvadeBack;
        StateMachine._playerController.InputSystem.OnBigSkillEvent += OnBigSkill;
        StateMachine._animator.Play("Idle");
    }


    public override void Update() { }

    public override void OnExit()
    {
        base.OnExit();
        StateMachine._playerController.InputSystem.OnMovePerformed -= OnMove;
        StateMachine._playerController.InputSystem.OnAttackEvent -= OnAttack;
        StateMachine._playerController.InputSystem.OnEvadeEvent -= OnEvadeBack;
        StateMachine._playerController.InputSystem.OnBigSkillEvent -= OnBigSkill;
    }
}