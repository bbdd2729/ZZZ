using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputSystem
{
    event Action<InputAction.CallbackContext> OnMovePerformed;
    event Action<InputAction.CallbackContext> OnMoveCanceled;
    event Action<InputAction.CallbackContext> OnEvadeEvent;
    event Action<InputAction.CallbackContext> OnWalkEvent;
    event Action<InputAction.CallbackContext> SwitchCharacterEvent;
    event Action<InputAction.CallbackContext> OnBigSkillEvent;
    event Action<InputAction.CallbackContext> OnAttackEvent;
    Vector2                                   MoveDirectionInput { get; }
    Vector2                                   CameraLook         { get; }
    bool                                      Run                { get; }
    bool                                      Crouch             { get; }
    bool                                      Walk               { get; }
    bool                                      Space              { get; }
    Vector2                   PlayerMove         { get; }
    void                      Init();
}