using UnityEngine;
using VContainer;

public class PlayerController : MonoBehaviour
{
    [SerializeField]                        private Animator            _animator;
    [SerializeField]                        private CharacterController _characterController;
    [SerializeField]                        private CameraSystem        _cameraSystem;
    public  float               RotationSpeed = 10f;
    [SerializeField]                        private Camera              _camera;
    public                                          StateMachine        _stateMachine;
    public                                          int                 AttackLength = 4;
    public                                          ScriptableObject    PlayerData;
    public                                          Transform           LookAtPoint;



    public Vector3    CamPosition => _cameraSystem.CamPosition;
    public Quaternion CamRotation => _cameraSystem.CamRotation;


    [Inject] private IStateMachineFactory _stateMachineFactory;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        // 使用工厂创建状态机（如果可用）
        if (_stateMachineFactory != null)
        {
            _stateMachine = _stateMachineFactory.CreateStateMachine(this) as StateMachine;
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
        _stateMachine = new StateMachine(this, _characterController, _animator);
        
        // 注册状态（保持原有逻辑）
        _stateMachine.RegisterState(new IdleState());
        _stateMachine.RegisterState(new WalkState());
        _stateMachine.RegisterState(new RunState());
        _stateMachine.RegisterState(new EvadeState());
        _stateMachine.RegisterState(new EvadeBackState());
        _stateMachine.RegisterState(new EvadeBackEndState());
        _stateMachine.RegisterState(new BigSkillState());
        _stateMachine.RegisterState(new AttackState());
        _stateMachine.RegisterState(new AttackEndState());
        _stateMachine.RegisterState(new SwitchInState());
        _stateMachine.RegisterState(new SwitchOutState());
        
        // 设置初始状态
        _stateMachine.ChangeState<IdleState>();
    }
    private void Start() { }

    private void Update()
    {
        //Debug.DrawRay(CamPosition, CamRotation * Vector3.forward, Color.red)


        _stateMachine.Update();
    }

    public void SetCharacterRotation()
    {
        var input = InputSystem.Instance.MoveDirectionInput;

        // 计算目标角度（基于摄像机朝向）
        var targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + CameraSystem.Instance.CamRotation.eulerAngles.y;

        // 平滑旋转角色
        var targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }


    private void OnEnable()
    {
        // OnEnable 时不自动启用输入，由状态机控制
        _stateMachine.Enable();
    }

    private void OnDisable()
    {
        _stateMachine.Disable();
        SetInputActive(false);
    }

    public void SetInputActive(bool value)
    {
        // 把你所有检测 Input.GetKey / ReadValue 的 flag 统一收拢到这里
        this.enabled = value;   // 直接关闭组件是最简单的做法
        
        // 确保状态机也正确启用或禁用
        if (value && this.gameObject.activeInHierarchy)
        {
            _stateMachine.Enable();
        }
        else
        {
            _stateMachine.Disable();
        }
    }
}