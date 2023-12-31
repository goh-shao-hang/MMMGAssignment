using GameCells.Modules;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class ThirdPersonMovement : MonoBehaviourPun
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        [SerializeField] private ThirdPersonCamera _playerCamera;
        [SerializeField] private Rigidbody _playerRigidbody;
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private CollisionCheckModule _groundCheck;
        [SerializeField] private Raycast3DModule _slopeCheck;
        [SerializeField] private Raycast3DModule _lowerStepCheck;
        [SerializeField] private Raycast3DModule _upperStepCheck;
        [SerializeField] private ParticleSystem _jumpParticles;

        [Header("Settings")]
        [SerializeField] private float _groundMoveSpeed = 25f;
        [SerializeField] private float _airMoveSpeed = 3f;
        [SerializeField] private float _maxSpeed = 6f;
        [SerializeField] private float _aimingMaxSpeed = 3f;
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _jumpSpeed = 15f;
        [SerializeField] private float _maxSlopeAngle = 45f;
        [SerializeField] private float _maxStepHeight = 0.3f;
        [SerializeField] private float _stepForce = 1f;
        [SerializeField] private float _groundDrag = 3f;
        [SerializeField] private float _airDrag = 0f;

        private Vector3 _moveInput;
        private Vector3 _targetDirection;
        private Vector3 _targetVelocity;

        private bool _isGroundedLastFrame;
        private bool _isOnSlope => Vector3.Angle(Vector3.up, _slopeCheck.HitInfo().normal) > 5f;
        private bool _isAiming;

        //EVENTS
        public event Action OnStartMoving;
        public event Action OnStopMoving;
        public event Action OnJump;

        private bool IsGrounded()
        {
            _isGroundedLastFrame = _groundCheck.Hit;
            return _isGroundedLastFrame;
        }

        private void OnEnable()
        {
            _playerInputHandler.JumpInput += HandleJump;

            _upperStepCheck.transform.localPosition = new Vector3(0f, _maxStepHeight, 0f);
        }

        private void OnDisable()
        {
            _playerInputHandler.JumpInput -= HandleJump;
        }

        private void Update()
        {
            HandleInput();

            if (!_isGroundedLastFrame && IsGrounded())
            {
                SpawnJumpParticles();
            }
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
            if (IsGrounded())
            {
                _playerRigidbody.drag = _groundDrag;
                _targetVelocity = _targetDirection * _groundMoveSpeed;
            }
            else
            {
                _playerRigidbody.drag = _airDrag;
                _targetVelocity = _targetDirection * _airMoveSpeed;
            }

            _targetVelocity.y = 0f;

            _playerRigidbody.AddForce(_targetVelocity);

            CheckSlopeMovement();

            CheckStepClimb();

            LimitMaxSpeed();

            UpdateMoveAnimationsAndAudio();
        }

        private void UpdateMoveAnimationsAndAudio()
        {
            if (_playerAnimator != null)
            {
                if (_playerRigidbody.velocity.sqrMagnitude > 0.1f)
                {
                    _playerAnimator.SetBool(GameData.IS_MOVING_HASH, true);

                    if (IsGrounded())
                    {
                        OnStartMoving?.Invoke();
                    }
                    else
                    {
                        OnStopMoving?.Invoke();
                    }
                }
                else
                {
                    _playerAnimator.SetBool(GameData.IS_MOVING_HASH, false);
                    OnStopMoving?.Invoke();
                }
            }
        }

        private void LimitMaxSpeed()
        {
            Vector3 currentSpeed = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);
            if (currentSpeed.magnitude > _maxSpeed)
            {
                _targetVelocity = currentSpeed.normalized * (_isAiming ? _aimingMaxSpeed : _maxSpeed);
                _targetVelocity.y = _playerRigidbody.velocity.y;
                _playerRigidbody.velocity = _targetVelocity;
            }   
        }

        private void CheckSlopeMovement()
        {
            if (_isOnSlope) //Stop gravity when on slope to prevent sliding down slopes
            {
                Vector3 slopeNormal = _slopeCheck.HitInfo().normal;

                if (Vector3.Angle(slopeNormal, Vector3.up) > _maxSlopeAngle || (_moveInput != Vector3.zero && Vector3.Angle(_targetDirection, slopeNormal) < 90f)) //Too steep or moving down slope. Added force avoids bump
                {
                    _playerRigidbody.AddForce(0f, -30f, 0f);
                    _playerRigidbody.useGravity = true;
                }
                else if (_playerRigidbody.velocity.y < 0)
                {
                    _playerRigidbody.useGravity = false;

                }
            }
            else
                _playerRigidbody.useGravity = true;
        }

        private void CheckStepClimb()
        {
            bool stepAhead = _lowerStepCheck.Hit && !_upperStepCheck.Hit;
            bool movingTowardsStep = _moveInput != Vector3.zero && Vector3.Angle(_targetDirection, _upperStepCheck.transform.forward) < 15f;

            if (stepAhead && !_isOnSlope && movingTowardsStep)
            {
                _playerRigidbody.AddForce(0f, _stepForce, 0f);
            }
        }

        private void HandleRotation()
        {
            Quaternion targetRotation;

            if (!_isAiming)
            {
                if (_targetDirection != Vector3.zero)
                    targetRotation = Quaternion.LookRotation(_targetDirection);
                else
                    targetRotation = Quaternion.LookRotation(transform.forward);
            }
            else
            {
                targetRotation = Quaternion.AngleAxis(_playerCamera.CameraFollowTarget.eulerAngles.y, Vector3.up);
            }

            Quaternion surfaceAlignmentRotation = Quaternion.FromToRotation(transform.up, _slopeCheck.HitInfo().normal);
            
            if (!_isAiming)
            {
                _playerRigidbody.rotation = Quaternion.Slerp(transform.rotation, surfaceAlignmentRotation * targetRotation, _rotationSpeed * Time.deltaTime);
            }
            else //Rotate immediately if aiming
            {
                _playerRigidbody.rotation = Quaternion.Slerp(transform.rotation, surfaceAlignmentRotation * targetRotation, _rotationSpeed * Time.deltaTime * 2);
            }
        }

        private void HandleJump()
        {
            if (IsGrounded())
            {
                _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0f, _playerRigidbody.velocity.z);
                _playerRigidbody.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);

                _playerAnimator.SetTrigger(GameData.JUMP_HASH);

                SpawnJumpParticles();
                OnJump?.Invoke();
            }
        }

        public void SetIsAiming(bool isAiming)
        {
            if (!photonView.IsMine)
                return;

            _isAiming = isAiming;
        }

        //TODO: particles with own script
        public void SpawnJumpParticles()
        {
            if (_jumpParticles != null)
            {
                GameObject particles = Instantiate(_jumpParticles, transform.position, Quaternion.identity).gameObject;
                Destroy(particles, 1f);
            }
        }
    }
}

public enum EMovementMode
{
    Velocity = 0,
    Force = 1
}
