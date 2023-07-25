using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;

namespace GameCells.Player
{
    public class ThirdPersonCamera : MonoBehaviourPun
    {
        [Header("Dependencies")]
        [SerializeField] private Canvas _hudCanvas;
        [SerializeField] private PlayerInputHandler _playerInputHandler;

        [Header("Settings")]
        [SerializeField] private float _xSensitivity = 100f;
        [SerializeField] private float _ySensitivity = 100f;
        [SerializeField] private float _maxLookUpAngle = 80f;
        [SerializeField] private float _maxLookDownAngle = -30f;
        [SerializeField] private bool _smooth;
        [SerializeField] private float _smoothFactor = 10f;
        [SerializeField] private bool _invertXAxis = false;
        [SerializeField] private bool _invertYAxis = true;

        [SerializeField] private CinemachineVirtualCamera _playerNormalCamera;
        [SerializeField] private CinemachineVirtualCamera _playerAimingCamera;

        private Transform _cameraFollowTarget = null;
        public Transform CameraFollowTarget => _cameraFollowTarget;

        private float _xRotation;
        private float _yRotation;

        //TODO
        //private bool IsUsingMouse =>

        private void Start()
        {
            _hudCanvas.gameObject.SetActive(false);
            

            if (_cameraFollowTarget == null)
            {
                _cameraFollowTarget = new GameObject("CameraFollowTarget").transform;
                _cameraFollowTarget.transform.position = transform.position + new Vector3(0f, 1f, 0f);

                _playerNormalCamera.m_Follow = _cameraFollowTarget;
                _playerAimingCamera.m_Follow = _cameraFollowTarget;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            _playerInputHandler.AimInputPressed += () => ToggleAim(true);
            _playerInputHandler.AimInputReleased += () => ToggleAim(false);
        }

        private void OnDisable()
        {
            _playerInputHandler.AimInputPressed -= () => ToggleAim(true);
            _playerInputHandler.AimInputReleased -= () => ToggleAim(false);
        }

        private void ToggleAim(bool aim)
        {
            if (!photonView.IsMine)
                return;

            //TODO: decouple hud
            _hudCanvas?.gameObject.SetActive(aim);
            _playerAimingCamera.Priority = aim ? 11 : 9;

            
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            _cameraFollowTarget.transform.position = transform.position + new Vector3(0f, 1f, 0f);

            if (_playerInputHandler.LookInput.sqrMagnitude >= 0.001f)
            {
                int invertX = _invertXAxis ? -1 : 1;
                int invertY = _invertYAxis ? -1 : 1;

                _yRotation += _playerInputHandler.LookInput.x * _xSensitivity * Time.deltaTime * invertX;
                _xRotation += _playerInputHandler.LookInput.y * _ySensitivity * Time.deltaTime * invertY;
            }

            _xRotation = Mathf.Clamp(_xRotation, _maxLookDownAngle, _maxLookUpAngle);

            // Cinemachine will follow this target
            if (!_smooth)
                _cameraFollowTarget.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0.0f);
            else
                _cameraFollowTarget.transform.rotation = Quaternion.Slerp(_cameraFollowTarget.transform.rotation, Quaternion.Euler(_xRotation, _yRotation, 0.0f), _smoothFactor * Time.deltaTime);
        }
    }
}