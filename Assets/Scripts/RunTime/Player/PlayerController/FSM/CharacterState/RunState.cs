public class RunState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine._animator.CrossFade("Run", StateMachine._playerController.AnimationTranslationTime);
        InputSystem.Instance.OnMoveCanceled += OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent += OnEvadeEvent;
        InputSystem.Instance.OnBigSkillEvent += OnBigSkill;
        InputSystem.Instance.OnAttackEvent += OnAttack;
    }

    public override void Update()
    {
        StateMachine._playerController.SetCharacterRotation();
    }

    public override void OnExit()
    {
        InputSystem.Instance.OnMoveCanceled -= OnMoveCanceled;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeEvent;
        InputSystem.Instance.OnBigSkillEvent -= OnBigSkill;
        InputSystem.Instance.OnAttackEvent -= OnAttack;
        base.OnExit();
    }
}