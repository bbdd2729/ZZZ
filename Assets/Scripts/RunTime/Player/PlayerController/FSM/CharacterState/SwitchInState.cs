
public class SwitchInState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        DebugX.Instance.Log($"SwitchInState OnEnter");
        StateMachine._animator.Play("SwitchIn_Normal");
        StateMachine.SetStateLocked(true);
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
        DebugX.Instance.Log($"SwitchInState OnExit");
        // 确保在切换入状态结束后启用角色控制器
        StateMachine._playerController.enabled = true;
        StateMachine._playerController.SetInputActive(true);
    }
}