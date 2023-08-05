using GameCells.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Optional")]
    [SerializeField] private Slider _mobileSensitivitySlider;

    //[Header("Settings")]
    //[SerializeField]
    private float _mobileLookSensitivity;

    public event Action<bool> OnMobileInputActiveStateChanged;

    public Vector2 MobileLookInput => _cameraRotateField.TouchDelta * _mobileLookSensitivity;
    public bool IsMobileInputActive { get; private set; }

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
            _mobileLookSensitivity = _mobileSensitivitySlider.value;
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
