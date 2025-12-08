using UnityEngine;

public class IdleState : BaseState
{
    public override void OnEnter()
    {
        // 播放闲置动画

        //InputSystem.Instance.OnMovePerformed += OnMove;
        InputSystem.Instance.OnAttackEvent += OnAttack;
        InputSystem.Instance.OnEvadeEvent += OnEvadeBack;
        InputSystem.Instance.OnBigSkillEvent += OnBigSkill;
        StateMachine._animator.CrossFade("Idle", StateMachine._playerController.AnimationTranslationTime);
    }


    public override void Update()
    {
        base.Update();
        if (InputSystem.Instance.PlayerMove != Vector2.zero) StateMachine.ChangeState<WalkState>();
    }

    public override void OnExit()
    {
        base.OnExit();
        //InputSystem.Instance.OnMovePerformed -= OnMove;
        InputSystem.Instance.OnAttackEvent -= OnAttack;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeBack;
        InputSystem.Instance.OnBigSkillEvent -= OnBigSkill;
    }
}