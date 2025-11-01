

using UnityEngine;

public class SwitchInState : BaseState
{
    private bool _switchComplete = false;
    private float _switchInDuration = 0.4f;
    private float _switchInTimer = 0f;

    public override void OnEnter()
    {
        base.OnEnter();
        StateMachine.StateLocked = true;
        _switchComplete = false;
        _switchInTimer = 0f;
        
        DebugX.Instance.Log($"SwitchInState OnEnter - Character: {StateMachine._playerController.name}");
        
        // 启用角色和控制器
        StateMachine._playerController.gameObject.SetActive(true);
        StateMachine._characterController.enabled = true;
        
        // 播放切入动画
        StateMachine._animator.Play("Switch_In");
        
        // 重置角色位置和状态
        ResetCharacterState();
    }

    public override void Update()
    {
        base.OnUpdate();
        
        if (_switchComplete) return;
        
        _switchInTimer += Time.deltaTime;
        
        // 使用时间或动画进度判断切入是否完成
        if (_switchInTimer >= _switchInDuration || IsAnimationEnd())
        {
            _switchComplete = true;
            CompleteSwitchIn();
        }
    }

    private void ResetCharacterState()
    {
        // 重置动画状态
        StateMachine._animator.SetFloat("MoveSpeed", 0f);
        StateMachine._animator.SetBool("IsMoving", false);
        
        // 重置状态机变量
        StateMachine.currentNormalAttackIndex = 1;
        
        // 通知摄像机系统切换目标
        CameraSystem.Instance?.SetTarget(StateMachine._playerController.transform);
    }

    private void CompleteSwitchIn()
    {
        DebugX.Instance.Log("SwitchInState Complete");
        
        // 解锁状态并切换到待机
        StateMachine.StateLocked = false;
        StateMachine.ChangeState<IdleState>();
    }

    public override void OnExit()
    {
        base.OnExit();
        
        // 确保状态解锁
        StateMachine.StateLocked = false;
        
        DebugX.Instance.Log($"SwitchInState OnExit - Character: {StateMachine._playerController.name}");
    }
}