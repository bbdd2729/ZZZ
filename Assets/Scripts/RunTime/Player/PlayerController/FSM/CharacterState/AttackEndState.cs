public class AttackEndState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();

        //播放攻击后摇动画
        StateMachine._animator.Play($"Attack_Normal_{StateMachine.currentNormalAttackIndex}_End");
        InputSystem.Instance.OnMovePerformed += OnMove;
        InputSystem.Instance.OnBigSkillEvent += OnBigSkill;
        InputSystem.Instance.OnEvadeEvent += OnEvadeEvent;
    }

    public override void Update()
    {
        base.Update();

        if (InputSystem.Instance.InputActions.Player.Attack.triggered)
        {
            //攻击段数累加
            StateMachine.currentNormalAttackIndex++;
            if (StateMachine.currentNormalAttackIndex > StateMachine._playerController.AttackLength)
                // 当前动机段数归零
                StateMachine.currentNormalAttackIndex = 1;

            //切换到普通攻击状态
            StateMachine.ChangeState<AttackState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        InputSystem.Instance.OnMovePerformed -= OnMove;
        InputSystem.Instance.OnBigSkillEvent -= OnBigSkill;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeEvent;
    }
}