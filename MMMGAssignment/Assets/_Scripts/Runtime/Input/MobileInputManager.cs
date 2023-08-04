using GameCells.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputManager : Singleton<MobileInputManager>
{
    [Header("Dependencies")]
    [SerializeField] private Canvas _mobileInputCanvas;
    [SerializeField] private GameObject _normalUI;
    [SerializeField] private GameObject _aimingUI;
    [SerializeField] private GameObject _aimButton;

    private bool _isMobileInputActive;

    private void Awake()
    {
        ActivateMobileInput(Application.isMobilePlatform);
        SetHasGun(false);
        SetIsAiming(false);
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.M))
        {
            ActivateMobileInput(!_isMobileInputActive);
            Debug.LogWarning($"Mobile Debug Mode {(_isMobileInputActive ? "On" : "Off")}");
        }

#endif
    }

    private void ActivateMobileInput(bool activate)
    {
        _isMobileInputActive = activate;
        _mobileInputCanvas.gameObject.SetActive(activate);
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
