


public class AttackEndState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();

        //播放攻击后摇动画
        StateMachine._animator.Play($"Attack_Normal_{ StateMachine.currentNormalAttackIndex }_End") ;
        StateMachine._playerController.InputSystem.OnMovePerformed += OnMove;
        StateMachine._playerController.InputSystem.OnBigSkillEvent += OnBigSkill;
        StateMachine._playerController.InputSystem.OnEvadeEvent += OnEvadeEvent;
    }

    public override void Update()
    {
        base.Update();

        if (StateMachine._playerController.InputSystem.InputActions.Player.Attack.triggered)
        {
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
    }

    public override void OnExit()
    {
        base.OnExit();
        StateMachine._playerController.InputSystem.OnMovePerformed -= OnMove;
        StateMachine._playerController.InputSystem.OnBigSkillEvent -= OnBigSkill;
        StateMachine._playerController.InputSystem.OnEvadeEvent -= OnEvadeEvent;
    }
}