

public class SwitchInState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        DebugX.Instance.Log($"SwitchInState OnEnter");
        StateMachine._animator.Play("Switch_In");

    }

    public override void Update()
    {
        base.Update();
        #region 检测动画是否结束
        if (IsAnimationEnd())
        {
            //切换到待机状态
            StateMachine.StateLocked = false;
            StateMachine.ChangeState<IdleState>();
            return;
        }
        #endregion
    }

    public override void OnExit()
    {
        base.OnExit();
        DebugX.Instance.Log($"SwitchInState OnExit");
    }
}