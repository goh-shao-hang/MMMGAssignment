using GameCells.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MobileInputManager : Singleton<MobileInputManager>
{
    [Header("Dependencies")]
    [SerializeField] private Canvas _mobileInputCanvas;
    [SerializeField] private GameObject _normalUI;
    [SerializeField] private GameObject _aimingUI;
    [SerializeField] private GameObject _aimButton;
    [SerializeField] private FixedTouchField _cameraRotateField;
    [SerializeField] private ShootButton _shootButton;

    [Header("Optional")]
    [SerializeField] private Slider _mobileSensitivitySlider;
    [SerializeField] private TMP_Text _mobileSensitivityText;
    [SerializeField] private float _shootJoystickSensitivty = 0.1f;

    public event Action<bool> OnMobileInputActiveStateChanged;
    public float ShootJoystickSensitivity => _shootJoystickSensitivty;

    public bool IsMobileInputActive { get; private set; }
    //When this is held, ignore camera rotation via touch field and use this joystick for cam rotation instead
    public bool IsShootButtonHeld => _shootButton.IsShootButtonHeld;

    public Vector2 TouchFieldInput => _cameraRotateField.TouchDelta * _mobileLookSensitivity;

    private float _mobileLookSensitivity;

    private void Start()
    {
        ActivateMobileInput(Application.isMobilePlatform);

        SetHasGun(false);
        SetIsAiming(false);

        UpdateMobileSensitivity();
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.M))
        {
            ActivateMobileInput(!IsMobileInputActive);
            Debug.LogWarning($"Mobile Debug Mode {(IsMobileInputActive ? "On" : "Off")}");
        }

#endif
    }

    public void UpdateMobileSensitivity()
    {
        if (_mobileSensitivitySlider != null)
        {
            _mobileLookSensitivity = _mobileSensitivitySlider.value * 0.01f;

            if (_mobileSensitivityText != null)
                _mobileSensitivityText.SetText($"Sensitivity: {_mobileSensitivitySlider.value}");
        }
    }

    private void ActivateMobileInput(bool activate)
    {
        IsMobileInputActive = activate;
        _mobileInputCanvas.gameObject.SetActive(activate);

        OnMobileInputActiveStateChanged?.Invoke(activate);
    }

    public void SetHasGun(bool hasGun)
    {
        _aimButton.SetActive(hasGun);
    }

    public void SetIsAiming(bool isAiming)
    {
        _normalUI.SetActive(!isAiming);
        _aimingUI.SetActive(isAiming);
    }
}
