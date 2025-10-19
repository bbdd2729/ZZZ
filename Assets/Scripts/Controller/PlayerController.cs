using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator            _animator;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CameraSystem        _cameraSystem;
    public                   float               rotationSpeed = 10f;
    [SerializeField] private Camera              _camera;
    private                  StateMachine        _stateMachine;

    public Vector3    CamPosition => _cameraSystem.CamPosition;
    public Quaternion CamRotation => _cameraSystem.CamRotation;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        InputSystem.Instance.InputActions.Enable();


        _stateMachine = new StateMachine(this, _characterController, _animator);
        _stateMachine.RegisterState(new IdleState());
        _stateMachine.RegisterState(new WalkState());
        _stateMachine.RegisterState(new RunState());
        _stateMachine.RegisterState(new EvadeState());
        _stateMachine.RegisterState(new EvadeBackState());
        _stateMachine.RegisterState(new BigSkillState());


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
        var targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + _cameraSystem.CamRotation.eulerAngles.y;

        // 平滑旋转角色
        var targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}