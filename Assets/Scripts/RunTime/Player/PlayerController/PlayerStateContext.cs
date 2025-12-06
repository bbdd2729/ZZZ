using UnityEngine;

/// <summary>
/// 玩家状态上下文
/// 封装状态机和状态所需的所有数据和方法
/// 降低StateMachine与PlayerController的直接耦合
/// </summary>
public class PlayerStateContext
{
    /// <summary>
    /// 角色动画组件
    /// </summary>
    public Animator Animator { get; private set; }
    
    /// <summary>
    /// 角色控制器组件
    /// </summary>
    public CharacterController CharacterController { get; private set; }
    
    /// <summary>
    /// 玩家控制器组件
    /// </summary>
    public PlayerController PlayerController { get; private set; }
    
    /// <summary>
    /// 输入系统实例
    /// </summary>
    public InputSystem InputSystem { get; private set; }
    
    /// <summary>
    /// 相机系统
    /// </summary>
    public CameraSystem CameraSystem { get; private set; }
    
    /// <summary>
    /// 初始化玩家状态上下文
    /// </summary>
    /// <param name="playerController">玩家控制器实例</param>
    public PlayerStateContext(PlayerController playerController)
    {
        PlayerController = playerController;
        Animator = playerController.GetComponent<Animator>();
        CharacterController = playerController.GetComponent<CharacterController>();
        InputSystem = InputSystem.Instance;
        CameraSystem = CameraSystem.Instance;
    }
    
    /// <summary>
    /// 设置角色旋转
    /// </summary>
    public void SetCharacterRotation()
    {
        PlayerController.SetCharacterRotation();
    }
    
    /// <summary>
    /// 获取相机位置
    /// </summary>
    public Vector3 CamPosition => PlayerController.CamPosition;
    
    /// <summary>
    /// 获取相机旋转
    /// </summary>
    public Quaternion CamRotation => PlayerController.CamRotation;
}