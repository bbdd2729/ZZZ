public class IdleState : BaseState
{
    public override void OnEnter()
    {
        // 播放闲置动画

        InputSystem.Instance.OnMovePerformed += OnMove;
        InputSystem.Instance.OnAttackEvent += OnAttack;
        InputSystem.Instance.OnEvadeEvent += OnEvadeBack;
        InputSystem.Instance.OnBigSkillEvent += OnBigSkill;
        StateMachine._animator.Play("Idle");
    }


    public override void Update() { }

    public override void OnExit()
    {
        base.OnExit();
        InputSystem.Instance.OnMovePerformed -= OnMove;
        InputSystem.Instance.OnAttackEvent -= OnAttack;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeBack;
        InputSystem.Instance.OnBigSkillEvent -= OnBigSkill;
    }
}