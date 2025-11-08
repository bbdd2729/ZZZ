

public class EvadeBackEndState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        DebugX.Instance.Log($"EvadeBackEndState OnEnter");
        StateMachine._animator.Play("Evade_Back_End");

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
        DebugX.Instance.Log($"EvadeBackEndState OnExit");

    }
}