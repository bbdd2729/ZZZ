using UnityEngine;

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


    private void Awake()
    {
        _animator = GetComponent<Animator>();


        _stateMachine = new StateMachine(this, _characterController, _animator);
        _stateMachine.RegisterState(new IdleState());
        _stateMachine.RegisterState(new WalkState());
        _stateMachine.RegisterState(new RunState());
        _stateMachine.RegisterState(new EvadeState());
        _stateMachine.RegisterState(new EvadeBackState());
        _stateMachine.RegisterState(new BigSkillState());
        _stateMachine.RegisterState(new AttackState());
        _stateMachine.RegisterState(new AttackEndState());
        _stateMachine.RegisterState(new EvadeBackEndState());



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
        _stateMachine.Enable();
        SetInputActive(true);
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
    }
}