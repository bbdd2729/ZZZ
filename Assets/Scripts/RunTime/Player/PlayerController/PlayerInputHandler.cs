using System;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家输入处理器
/// 集中管理输入事件，减少状态类中的输入事件注册/取消注册代码
/// 提高代码的可维护性和可扩展性
/// </summary>
public class PlayerInputHandler
{
    /// <summary>
    /// 输入系统实例
    /// </summary>
    private InputSystem _inputSystem;
    
    /// <summary>
    /// 移动事件委托
    /// </summary>
    public Action<InputAction.CallbackContext> OnMove { get; set; }
    
    /// <summary>
    /// 移动取消事件委托
    /// </summary>
    public Action<InputAction.CallbackContext> OnMoveCanceled { get; set; }
    
    /// <summary>
    /// 闪避事件委托
    /// </summary>
    public Action<InputAction.CallbackContext> OnEvade { get; set; }
    
    /// <summary>
    /// 后闪避事件委托
    /// </summary>
    public Action<InputAction.CallbackContext> OnEvadeBack { get; set; }
    
    /// <summary>
    /// 攻击事件委托
    /// </summary>
    public Action<InputAction.CallbackContext> OnAttack { get; set; }
    
    /// <summary>
    /// 大技能事件委托
    /// </summary>
    public Action<InputAction.CallbackContext> OnBigSkill { get; set; }
    
    /// <summary>
    /// 初始化输入处理器
    /// </summary>
    /// <param name="inputSystem">输入系统实例</param>
    public PlayerInputHandler(InputSystem inputSystem)
    {
        _inputSystem = inputSystem;
        RegisterInputEvents();
    }
    
    /// <summary>
    /// 注册输入事件
    /// </summary>
    private void RegisterInputEvents()
    {
        _inputSystem.OnMovePerformed += HandleMove;
        _inputSystem.OnMoveCanceled += HandleMoveCanceled;
        _inputSystem.OnEvadeEvent += HandleEvade;
        _inputSystem.OnBigSkillEvent += HandleBigSkill;
        _inputSystem.OnAttackEvent += HandleAttack;
    }
    
    /// <summary>
    /// 取消注册输入事件
    /// </summary>
    public void UnregisterInputEvents()
    {
        _inputSystem.OnMovePerformed -= HandleMove;
        _inputSystem.OnMoveCanceled -= HandleMoveCanceled;
        _inputSystem.OnEvadeEvent -= HandleEvade;
        _inputSystem.OnBigSkillEvent -= HandleBigSkill;
        _inputSystem.OnAttackEvent -= HandleAttack;
    }
    
    /// <summary>
    /// 处理移动事件
    /// </summary>
    /// <param name="ctx">输入事件上下文</param>
    private void HandleMove(InputAction.CallbackContext ctx)
    {
        OnMove?.Invoke(ctx);
    }
    
    /// <summary>
    /// 处理移动取消事件
    /// </summary>
    /// <param name="ctx">输入事件上下文</param>
    private void HandleMoveCanceled(InputAction.CallbackContext ctx)
    {
        OnMoveCanceled?.Invoke(ctx);
    }
    
    /// <summary>
    /// 处理闪避事件
    /// </summary>
    /// <param name="ctx">输入事件上下文</param>
    private void HandleEvade(InputAction.CallbackContext ctx)
    {
        OnEvade?.Invoke(ctx);
    }
    
    /// <summary>
    /// 处理大技能事件
    /// </summary>
    /// <param name="ctx">输入事件上下文</param>
    private void HandleBigSkill(InputAction.CallbackContext ctx)
    {
        OnBigSkill?.Invoke(ctx);
    }
    
    /// <summary>
    /// 处理攻击事件
    /// </summary>
    /// <param name="ctx">输入事件上下文</param>
    private void HandleAttack(InputAction.CallbackContext ctx)
    {
        OnAttack?.Invoke(ctx);
    }
    
    /// <summary>
    /// 清理资源
    /// </summary>
    public void Dispose()
    {
        UnregisterInputEvents();
    }
}