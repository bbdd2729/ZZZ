using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : SingletonBase<InputSystem>
{
    public InputSystem_Actions InputActions;

    public InputSystem()
    {
        Debug.Log("已创建InputSystem实例");
        if (InputActions == null) InputActions = new InputSystem_Actions();

        Init();
    }

    #region 回调事件配置

    /*private void SetupInputCallBacks()
    {
        InputActions.Player.Move.performed += ctx => OnMovePerformed?.Invoke(ctx);       //移动回调事件
        InputActions.Player.Move.canceled += ctx => OnMoveCanceled?.Invoke(ctx);         //移动取消回调事件
        InputActions.Player.Walk.performed += ctx => OnWalkEvent?.Invoke(ctx);           //行走回调事件
        InputActions.Player.Run.performed += ctx => OnEvadeEvent?.Invoke(ctx);           //奔跑冲刺回调事件
        InputActions.Player.Space.performed += ctx => SwitchCharacterEvent?.Invoke(ctx); //角色切换回调事件
        InputActions.Player.Attack.performed += ctx => OnAttackEvent?.Invoke(ctx);
    }*/

    #endregion

    #region InputAction输入事件定义

    public event Action<InputAction.CallbackContext> OnMovePerformed;

    public event Action<InputAction.CallbackContext> OnMoveCanceled;

    public event Action<InputAction.CallbackContext> OnEvadeEvent;

    public event Action<InputAction.CallbackContext> OnWalkEvent;

    public event Action<InputAction.CallbackContext> SwitchCharacterEvent;

    public event Action<InputAction.CallbackContext> OnBigSkillEvent;

    public event Action<InputAction.CallbackContext> OnAttackEvent;

    #endregion

    #region 系统初始化

    /*private void IntializeInputAction()
    {
        InputActions = new InputSystem_Actions();

        SetupInputCallBacks();

        InputActions.Enable();
    }*/

    private void Init()
    {
        InputActions.Player.Move.performed += ctx =>
        {
            OnMovePerformed?.Invoke(ctx);
            GameEvents.OnInput?.OnNext(new InputEvent());
        };                                                                       //移动回调事件
        InputActions.Player.Move.canceled += ctx => OnMoveCanceled?.Invoke(ctx); //移动取消回调事件
        InputActions.Player.Walk.performed += ctx => OnWalkEvent?.Invoke(ctx);   //行走回调事件
        InputActions.Player.Run.performed += ctx => OnEvadeEvent?.Invoke(ctx);   //奔跑冲刺回调事件
        InputActions.Player.Attack.performed += ctx => OnAttackEvent?.Invoke(ctx);
        InputActions.Player.Space.performed += ctx => SwitchCharacterEvent?.Invoke(ctx); //角色切换回调事件
        InputActions.Player.BigSkill.performed += ctx => OnBigSkillEvent?.Invoke(ctx);   //大技能回调事件
    }

    #endregion

    #region 键盘输入属性

    public Vector2 MoveDirectionInput => InputActions.Player.Move.ReadValue<Vector2>(); //获取移动方向   只读属性

    public Vector2 CameraLook => InputActions.Player.Look.ReadValue<Vector2>(); //获取相机方向   只读属性

    public bool Run => InputActions.Player.Run.triggered;

    public bool Crouch => InputActions.Player.Crouch.phase == InputActionPhase.Performed;

    public bool Walk => InputActions.Player.Walk.triggered;

    public bool Space => InputActions.Player.Space.triggered;

    public Vector2 PlayerMove => InputActions.Player.Move.ReadValue<Vector2>(); // 获取玩家移动输入向量

    #endregion
}