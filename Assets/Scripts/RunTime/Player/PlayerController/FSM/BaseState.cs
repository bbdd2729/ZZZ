using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseState : IState, IDisposable
{
    protected StateMachine StateMachine { get; private set; }

    protected AnimatorStateInfo  StateInfo { get; private set; }
    public virtual void Dispose() { }

    public virtual void OnEnter() { }

    public virtual void Update() { }

    public virtual void OnExit() { }

    public void Initialize (StateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public bool IsAnimationEnd()
    {
        StateInfo = StateMachine._animator.GetCurrentAnimatorStateInfo(0);

        return StateInfo.normalizedTime >= 1.0f && !StateMachine._animator.IsInTransition(0);
    }

    public float NormalizedTime()
    {
        //刷新动画状态
        StateInfo = StateMachine._animator.GetCurrentAnimatorStateInfo(0);

        return StateInfo.normalizedTime;
    }


    #region 事件处理函数

    protected void OnMove(InputAction.CallbackContext ctx)
    {
        // 切换到移动状态
        DebugX.Instance.Log("移动事件触发");
        StateMachine.ChangeState<WalkState>();
    }

    protected void OnEvadeBack(InputAction.CallbackContext ctx)
    {
        // 切换到跑步状态
        DebugX.Instance.Log("跑步/冲刺事件触发");
        StateMachine.ChangeState<EvadeBackState>();
    }

    protected void OnBigSkill(InputAction.CallbackContext ctx)
    {
        // 切换到大技能状态
        DebugX.Instance.Log("大技能事件触发");
        StateMachine.ChangeState<BigSkillState>();
    }

    protected void OnEvadeEvent(InputAction.CallbackContext ctx)
    {
        // 切换到跑步状态
        DebugX.Instance.Log("跑步/冲刺事件触发");
        StateMachine.ChangeState<EvadeState>();
    }

    protected void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        // 切换到闲置状态
        DebugX.Instance.Log("移动取消事件触发");
        StateMachine.ChangeState<IdleState>();
    }

    protected void OnAttack(InputAction.CallbackContext ctx)
    {
        // 切换到攻击状态
        DebugX.Instance.Log("攻击事件触发");
        StateMachine.ChangeState<AttackState>();
    }


    #endregion
}