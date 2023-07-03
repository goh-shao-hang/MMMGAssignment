using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class ThirdPersonMovement : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        [SerializeField] private Transform _playerCameraTransform;
        [SerializeField] private Rigidbody _playerRigidbody;

        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 7f;
        [SerializeField] private float _rotationSpeed = 15f;

        private Vector3 _targetVelocity;

        private void Update()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            _targetVelocity = _playerCameraTransform.forward * _playerInputHandler.MoveInput.y + _playerCameraTransform.right * _playerInputHandler.MoveInput.x;
            _targetVelocity *= _moveSpeed;
            _targetVelocity.y = _playerRigidbody.velocity.y;

            _playerRigidbody.velocity = _targetVelocity;
        }

        private void HandleRotation()
        {
            Vector3 targetDirection = Vector3.zero;

            targetDirection = _playerCameraTransform.forward * _playerInputHandler.MoveInput.y + _playerCameraTransform.right * _playerInputHandler.MoveInput.x;
            targetDirection.y = 0f;

            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            _playerRigidbody.rotation = playerRotation;
        }
    }
}