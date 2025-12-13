using UnityEngine;

public class WalkState : BaseState
{   
    
    private UniTaskTimer _timer;
    public override void OnEnter()
    {
        StateMachine._animator.CrossFade("Walk", StateMachine._playerController.AnimationTranslationTime);
        //InputSystem.Instance.OnMoveCanceled += OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent += OnEvadeEvent;
        InputSystem.Instance.OnBigSkillEvent += OnBigSkill;
        InputSystem.Instance.OnAttackEvent += OnAttack;
        _timer = UniTaskTimer.StartTimer(UniTaskTimer.Mode.Once, 6.0f,
                                UniTaskTimer.TimeSource.Scaled,
                                () => { StateMachine.ChangeState<RunState>(); }
                                );
    }

    public override void Update()
    {
        StateMachine._playerController.SetCharacterRotation();

        if (InputSystem.Instance.PlayerMove == Vector2.zero) StateMachine.ChangeState<IdleState>();
    }

    public override void OnExit()
    {
        base.OnExit();
        _timer.Stop();
        //InputSystem.Instance.OnMoveCanceled -= OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeEvent;
        InputSystem.Instance.OnBigSkillEvent -= OnBigSkill;
        InputSystem.Instance.OnAttackEvent -= OnAttack;
    }
}