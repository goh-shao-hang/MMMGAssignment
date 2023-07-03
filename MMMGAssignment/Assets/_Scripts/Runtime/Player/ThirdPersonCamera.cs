using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCells.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        [SerializeField] private Transform _followTarget;

        [Header("Settings")]
        [SerializeField] private float _xSensitivity = 100f;
        [SerializeField] private float _ySensitivity = 100f;
        [SerializeField] private float _maxLookUpAngle = 80f;
        [SerializeField] private float _maxLookDownAngle = -30f;
        [SerializeField] private bool _invertXAxis = false;
        [SerializeField] private bool _invertYAxis = true;

        private float _xRotation;
        private float _yRotation;

        private int _invertX => _invertXAxis ? -1 : 1;
        private int _invertY => _invertYAxis ? -1 : 1;

        //private bool IsUsingMouse =>

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            _followTarget.transform.position = transform.position + new Vector3(0f, 1f, 0f);

            if (_playerInputHandler.LookInput.sqrMagnitude >= 0.001f)
            {
                _yRotation += _playerInputHandler.LookInput.x * _xSensitivity * Time.deltaTime * _invertX;
                _xRotation += _playerInputHandler.LookInput.y * _ySensitivity * Time.deltaTime * _invertY;
            }

            _xRotation = Mathf.Clamp(_xRotation, _maxLookDownAngle, _maxLookUpAngle);

            _followTarget.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0.0f);
        }
    }
}