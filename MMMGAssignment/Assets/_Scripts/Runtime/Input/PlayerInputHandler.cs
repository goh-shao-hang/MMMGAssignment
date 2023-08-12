using GameCells.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _playerControls;

    private MobileInputManager _mobileInputManager;
    private MobileInputManager mobileInputManager => _mobileInputManager ??= MobileInputManager.GetInstance();

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public event Action JumpInput;
    public event Action HoldAimStarted;
    public event Action HoldAimEnded;
    public event Action ToggleAimInput;
    public event Action FireInputPressed;
    public event Action FireInputReleased;

    private bool _isMobileInputActive;

    public bool IsUsingMouse => Mouse.current.IsActuated();

    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            BindInput();
        }

        _playerControls.Enable();

        ChangeMobileInputActiveState(mobileInputManager.IsMobileInputActive);
        mobileInputManager.OnMobileInputActiveStateChanged += ChangeMobileInputActiveState;
    }

    

    private void OnDisable()
    {
        _playerControls.Disable();

        mobileInputManager.OnMobileInputActiveStateChanged -= ChangeMobileInputActiveState;
    }

    private void Update()
    {
        if (!_isMobileInputActive)
            return;

        if (!mobileInputManager.IsShootButtonHeld) //If shoot button is held, the shooting joystick will handle look input instead
        {
            LookInput = mobileInputManager.TouchFieldInput;
        }
    }


    private void BindInput()
    {
        _playerControls.Gameplay.Move.performed += ctx => OnMoveInput(ctx);
        _playerControls.Gameplay.Look.performed += ctx => OnLookInput(ctx);
        _playerControls.Gameplay.Jump.performed += ctx => OnJumpInput(ctx);
        _playerControls.Gameplay.HoldAim.performed += ctx => OnHoldAimInputStarted(ctx);
        _playerControls.Gameplay.HoldAim.canceled += ctx => OnHoldAimInputEnded(ctx);
        _playerControls.Gameplay.ToggleAim.performed += ctx => OnToggleAimInput(ctx);
        _playerControls.Gameplay.Fire.performed += ctx => OnFireInputPressed(ctx);
        _playerControls.Gameplay.Fire.canceled += ctx => OnFireInputReleased(ctx);
    }

    private void ChangeMobileInputActiveState(bool active)
    {
        _isMobileInputActive = active;
        LookInput = Vector2.zero;
    }

    private void OnMoveInput(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnLookInput(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();

        //If using shoot button to rotate camera, handle the sensitivity
        if (_isMobileInputActive && mobileInputManager.IsShootButtonHeld)
        {
            LookInput *= mobileInputManager.ShootJoystickSensitivity;
        }
    }

    private void OnJumpInput(InputAction.CallbackContext ctx)
    {
        JumpInput?.Invoke();
    }

    private void OnHoldAimInputStarted(InputAction.CallbackContext ctx)
    {
        HoldAimStarted?.Invoke();
    }

    private void OnHoldAimInputEnded(InputAction.CallbackContext ctx)
    {
        HoldAimEnded?.Invoke();
    }

    private void OnToggleAimInput(InputAction.CallbackContext ctx)
    {
        ToggleAimInput?.Invoke();
    }

    private void OnFireInputPressed(InputAction.CallbackContext ctx)
    {
        FireInputPressed?.Invoke();
    }

    private void OnFireInputReleased(InputAction.CallbackContext ctx)
    {
        FireInputReleased?.Invoke();
    }
}
