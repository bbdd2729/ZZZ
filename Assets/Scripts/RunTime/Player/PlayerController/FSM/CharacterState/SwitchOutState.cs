public class SwitchOutState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        DebugX.Instance.Log($"SwitchOutState OnEnter");
        StateMachine._animator.Play("SwitchOut_Normal");
        StateMachine.SetStateLocked(true);
        
        // 立即禁用输入控制
        StateMachine._playerController.SetInputActive(false);
    }

    public override void Update()
    {
        base.Update();
        #region 检测动画是否结束
        if (IsAnimationEnd())
        {
            //切换到待机状态
            StateMachine.SetStateLocked(false);
            StateMachine.ChangeState<IdleState>();
            return;
        }
        #endregion
    }

    public override void OnExit()
    {
        base.OnExit();
        DebugX.Instance.Log($"SwitchOutState OnExit");
    }
}