public class EvadeBackState : BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        StateMachine._animator.Play("Evade_Back");

    }

    public override void Update()
    {
        base.Update();
        #region 检测动画是否结束
        if (IsAnimationEnd())
        {
            //切换到待机状态
            StateMachine.StateLocked = false;
            StateMachine.ChangeState<EvadeBackEndState>();
            return;
        }
        #endregion
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}