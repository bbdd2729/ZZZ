﻿public class WalkState : BaseState
{
    public override void OnEnter()
    {
        StateMachine._animator.Play("Walk");
        InputSystem.Instance.OnMoveCanceled += OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent += OnEvadeEvent;
        InputSystem.Instance.OnBigSkillEvent += OnBigSkill;
        InputSystem.Instance.OnAttackEvent += OnAttack;
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
        InputSystem.Instance.OnMoveCanceled -= OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeEvent;
        InputSystem.Instance.OnBigSkillEvent -= OnBigSkill;
        InputSystem.Instance.OnAttackEvent -= OnAttack;
    }
}