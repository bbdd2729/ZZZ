using UnityEngine;
using VContainer;

public class PlayerController : MonoBehaviour
{
    [Header("角色基本组件和属性")]
    [SerializeField] internal Animator            _animator;
    [SerializeField] internal CharacterController    _characterController;
    [SerializeField] private  PlayerConfig           playerConfig;
    [SerializeField] private  PlayerControllerConfig playerControllerConfig;
    
    
    
    
    
    
    public                    ScriptableObject    PlayerData;
    public                    Transform           LookAtPoint;
    
    [Inject] private          CameraSystem        _cameraSystem;
    [Inject] private          IStateMachineFactory _stateMachineFactory;
    internal InputSystem  InputSystem { get => InputSystem.Instance; }
    
    public                   float        RotationSpeed {get=> playerControllerConfig.RotationSpeed;}
    public                   int          AttackLength {get=> playerConfig.AttackLength;}

    public StateMachine StateMachine { get; set; }

    


    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        // 使用工厂创建状态机（如果可用）
        if (_stateMachineFactory != null)
        {
            StateMachine = _stateMachineFactory.CreateStateMachine(this) as StateMachine;
        }
        else
        {
            // 回退到原有创建方式，保持兼容性
            CreateStateMachineManually();
        }
    }
    
    private void CreateStateMachineManually()
    {
        // 原有状态机创建逻辑
        StateMachine = new StateMachine(this);
        
        // 注册状态（保持原有逻辑）
        StateMachine.RegisterState(new IdleState());
        StateMachine.RegisterState(new WalkState());
        StateMachine.RegisterState(new RunState());
        StateMachine.RegisterState(new EvadeState());
        StateMachine.RegisterState(new EvadeBackState());
        StateMachine.RegisterState(new EvadeBackEndState());
        StateMachine.RegisterState(new BigSkillState());
        StateMachine.RegisterState(new AttackState());
        StateMachine.RegisterState(new AttackEndState());
        StateMachine.RegisterState(new SwitchInState());
        StateMachine.RegisterState(new SwitchOutState());
        
        // 设置初始状态
        StateMachine.ChangeState<IdleState>();
    }
    private void Start() { }

    private void Update()
    {
        //Debug.DrawRay(CamPosition, CamRotation * Vector3.forward, Color.red)


        StateMachine.Update();
    }

    public void SetCharacterRotation()
    {
        var input = InputSystem.MoveDirectionInput;

        // 计算目标角度（基于摄像机朝向）
        var targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + CameraSystem.Instance.CamRotation.eulerAngles.y;

        // 平滑旋转角色
        var targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }
    
    private void OnEnable()
    {
        // OnEnable 时不自动启用输入，由状态机控制
        StateMachine.Enable();
    }
    
    private void OnDisable()
    {
        StateMachine.Disable();
        SetInputActive(false);
    }

    public void SetInputActive(bool value)
    {
        // 把你所有检测 Input.GetKey / ReadValue 的 flag 统一收拢到这里
        this.enabled = value;   // 直接关闭组件是最简单的做法
        
        // 确保状态机也正确启用或禁用
        if (value && this.gameObject.activeInHierarchy)
        {
            StateMachine.Enable();
        }
        else
        {
            StateMachine.Disable();
        }
    }
    
    
    
    public void PlayerExit()
    {
        // 禁用输入
        SetInputActive(false);
        // 禁用状态机
        StateMachine.Disable();
    }
    
    public void PlayerEnter()
    {
        // 启用输入
        SetInputActive(true);
        // 启用状态机
        StateMachine.Enable();
    }
}