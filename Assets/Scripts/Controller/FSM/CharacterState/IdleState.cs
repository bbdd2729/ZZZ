public class IdleState : BaseState
{
    public override void OnEnter()
    {
        // 播放闲置动画
        StateMachine._animator.Play("Idle");
        InputSystem.Instance.OnMovePerformed += OnMove;
        InputSystem.Instance.OnEvadeEvent += OnEvadeBack;
    }


    public override void Update() { }

    public override void OnExit()
    {
        base.OnExit();
        InputSystem.Instance.OnMovePerformed -= OnMove;
        InputSystem.Instance.OnEvadeEvent -= OnEvadeBack;
    }
}