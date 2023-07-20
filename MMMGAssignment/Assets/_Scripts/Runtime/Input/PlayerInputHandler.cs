using GameCells.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _playerControls;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public event Action JumpInput;
    public event Action AimInputPressed;
    public event Action AimInputReleased;

    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            BindInput();
        }

        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void BindInput()
    {
        _playerControls.Gameplay.Move.performed += ctx => OnMoveInput(ctx);
        _playerControls.Gameplay.Look.performed += ctx => OnLookInput(ctx);
        _playerControls.Gameplay.Jump.performed += ctx => OnJumpInput(ctx);
        _playerControls.Gameplay.Aim.performed += ctx => OnAimInputPressed(ctx);
        _playerControls.Gameplay.Aim.canceled += ctx => OnAimInputReleased(ctx);
    }

    private void OnMoveInput(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnLookInput(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
    }

    private void OnJumpInput(InputAction.CallbackContext ctx)
    {
        JumpInput?.Invoke();
    }

    private void OnAimInputPressed(InputAction.CallbackContext ctx)
    {
        AimInputPressed?.Invoke();
    }

    private void OnAimInputReleased(InputAction.CallbackContext ctx)
    {
        AimInputReleased?.Invoke();
    }
}
