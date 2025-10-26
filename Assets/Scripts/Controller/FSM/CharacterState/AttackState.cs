public class AttackState : BaseState
{
    private bool enterNextAttack;
    public override void OnEnter()
        {
            base.OnEnter();
            enterNextAttack = false;

            StateMachine._animator.Play($"Attack_Normal_{ StateMachine.currentNormalAttackIndex }");
            InputSystem.Instance.OnMovePerformed += OnMove;
            InputSystem.Instance.OnBigSkillEvent += OnBigSkill;
        }

        public override void Update()
        {
            base.Update();

            if (NormalizedTime() >= 0.5f && InputSystem.Instance.InputActions.Player.Attack.triggered)
            {
                enterNextAttack = true;
            }

            #region 动画是否播放结束
            if (IsAnimationEnd())
            {
                if (enterNextAttack)
                {
                    // 切换到下一个普攻
                    //攻击段数累加
                    StateMachine.currentNormalAttackIndex++;
                    if (StateMachine.currentNormalAttackIndex > StateMachine._playerController.AttackLength)
                    {
                        // 当前动机段数归零
                        StateMachine.currentNormalAttackIndex = 1;
                    }

                    //切换到普通攻击状态
                    StateMachine.ChangeState<AttackState>();
                    return;
                }
                else
                {
                    //切换到普通攻击后摇状态
                    StateMachine.ChangeState<AttackEndState>();
                    return;
                }
            }
            #endregion

        }
        public override void OnExit()
        {
            StateMachine._animator.Play("Attack_Normal_1_End");
            base.OnExit();
            InputSystem.Instance.OnMovePerformed -= OnMove;
            InputSystem.Instance.OnBigSkillEvent -= OnBigSkill;
        }



}