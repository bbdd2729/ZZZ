using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseState : IState, IDisposable
{
    protected StateMachine        StateMachine        { get; private set; }
    protected PlayerController    PlayerController    { get; private set; }
    protected CharacterController CharacterController { get; private set; }
    protected Animator            Animator            { get; private set; }

    public virtual void Dispose() { }

    public virtual void OnEnter() { }

    public virtual void Update() { }

    public virtual void OnExit() { }


    // protected void PlayAnimation(string animationName)
    // {
    //
    // }
    //

    public void Initialize(PlayerController playerController, StateMachine stateMachine, CharacterController characterController,
                           Animator animator)
    {
        PlayerController = playerController;
        StateMachine = stateMachine;
        CharacterController = characterController;
        Animator = animator;
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

    protected void OnBigSkillEvent(InputAction.CallbackContext ctx)
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

    #endregion
}