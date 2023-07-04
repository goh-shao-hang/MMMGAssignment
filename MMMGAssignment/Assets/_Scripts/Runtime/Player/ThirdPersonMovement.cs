using GameCells.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class ThirdPersonMovement : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        [SerializeField] private ThirdPersonCamera _playerCamera;
        [SerializeField] private Rigidbody _playerRigidbody;
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private CollisionCheckModule _groundCheck;
        [SerializeField] private Raycast3DModule _slopeCheck;

        [Header("Settings")]
        [SerializeField] private float _groundMoveSpeed = 7f;
        [SerializeField] private float _airMoveSpeed = 3f;
        [SerializeField] private float _maxSpeed = 6f;
        [SerializeField] private float _airRotateCompensationMultiplier = 1.5f; 
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _jumpSpeed = 15f;
        [SerializeField] private float _maxSlopeAngle = 45f;
        [SerializeField] private float _groundDrag = 3f;
        [SerializeField] private float _airDrag = 0f;

        private Vector3 _moveInput;
        private Vector3 _targetDirection;
        private Vector3 _targetVelocity;

        private bool _isGrounded => _groundCheck.Hit;
        private bool _isOnSlope => Vector3.Angle(Vector3.up, _slopeCheck.HitInfo().normal) > 0.01f;

        private void OnEnable()
        {
            _playerInputHandler.JumpInput += HandleJump;
        }

        private void OnDisable()
        {
            _playerInputHandler.JumpInput -= HandleJump;
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleInput()
        {
            _moveInput.Set(_playerInputHandler.MoveInput.x, 0f, _playerInputHandler.MoveInput.y);

            _targetDirection = Quaternion.AngleAxis(_playerCamera.CameraFollowTarget.eulerAngles.y, Vector3.up) * _moveInput;
        }

        private void HandleMovement()
        {
            if (_isGrounded)
            {
                _playerRigidbody.drag = _groundDrag;
                _targetVelocity = _targetDirection * _groundMoveSpeed;
            }
            else
            {
                _playerRigidbody.drag = _airDrag;
                _targetVelocity = _targetDirection * _airMoveSpeed;

                float align = -(Vector3.Dot(_targetVelocity.normalized, _playerRigidbody.velocity.normalized) / _targetVelocity.normalized.sqrMagnitude);
                if (float.IsNaN(align))
                {
                    align = 0f;
                }
                align = Mathf.Clamp01(align);

                _targetVelocity = _targetVelocity + (_targetVelocity * _airRotateCompensationMultiplier * align);
            }

            _targetVelocity.y = 0f;

            _playerRigidbody.AddForce(_targetVelocity);

            if (_isOnSlope && _playerRigidbody.velocity.y < 0) //Stop gravity when on slope to prevent sliding down slopes
            {
                Vector3 slopeNormal = _slopeCheck.HitInfo().normal;
                    Debug.Log(Vector3.Angle(slopeNormal, Vector3.up));
                if (_moveInput != Vector3.zero && Vector3.Angle(_targetDirection.normalized, slopeNormal) < 90f) //Moving down slope. Adding force to avoid bump.
                    _playerRigidbody.AddForce(new Vector3(0f, -15f, 0f));
                else if (Vector3.Angle(slopeNormal, Vector3.up) < _maxSlopeAngle)
                    _playerRigidbody.useGravity = false;
                else
                    _playerRigidbody.useGravity = true;
            }
            else
                _playerRigidbody.useGravity = true;

            float currentSpeed = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z).magnitude;
            if (currentSpeed > _maxSpeed)
            {
                _targetVelocity = _targetDirection * _maxSpeed;
                _targetVelocity.y = _playerRigidbody.velocity.y;
                _playerRigidbody.velocity = _targetVelocity;
            }

            //Animations
            if (_playerAnimator != null)
            {
                if (_playerRigidbody.velocity.sqrMagnitude > 0.1f)
                {
                    _playerAnimator.SetBool(GameData.IS_MOVING_HASH, true);
                }
                else
                {
                    _playerAnimator.SetBool(GameData.IS_MOVING_HASH, false);
                }
            }
        }

        private void HandleRotation()
        {
            Quaternion targetRotation;
            if (_targetDirection != Vector3.zero)
                targetRotation = Quaternion.LookRotation(_targetDirection);
            else
                targetRotation = Quaternion.LookRotation(transform.forward);

            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            _playerRigidbody.rotation = playerRotation;
        }

        private void HandleJump()
        {
            if (_isGrounded)
            {
                _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);
                _playerRigidbody.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);

                _playerAnimator.SetTrigger(GameData.JUMP_HASH);
            }
        }
    }
}

public enum EMovementMode
{
    Velocity = 0,
    Force = 1
}
