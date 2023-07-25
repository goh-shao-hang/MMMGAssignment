using GameCells.Utilities;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace GameCells.Player
{
    public class PlayerShooting : MonoBehaviourPun
    {
        [Header("Dependencies")]
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _fireTransform;
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        [SerializeField] private GameObject _playerGun;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private TwoBoneIKConstraint _aimingArmRig;
        [SerializeField] private Transform _ikTargetTransform;

        [Header("Settings")]
        [SerializeField] private float _fireCD = 0.5f;

        private float _fireTimer = 0f;
        private bool _isAiming = false;
        private bool _isShooting = false;
        private Vector3 _bulletTarget;
        private RaycastHit _hit;
        private Coroutine _startAimingCO;

        private void Start()
        {
            if (_aimingArmRig != null)
            {
                _aimingArmRig.weight = 0;
            }

            StopAiming();
        }

        private void OnEnable()
        {
            _playerInputHandler.AimInputPressed += StartAiming;
            _playerInputHandler.AimInputReleased += StopAiming;
            _playerInputHandler.FireInputPressed += StartShooting;
            _playerInputHandler.FireInputReleased += StopShooting;
        }

        private void OnDisable()
        {
            _playerInputHandler.AimInputPressed -= StartAiming;
            _playerInputHandler.AimInputReleased -= StopAiming;
            _playerInputHandler.FireInputPressed -= StartShooting;
            _playerInputHandler.FireInputReleased -= StopShooting;
        }

        private void Update()
        {
            HandleGunRotation();
            HandleShooting();
        }

        private void StartAiming()
        {
            if (!photonView.IsMine)
                return;

            photonView.RPC(nameof(RPC_StartAiming), RpcTarget.All, true);

            if (_startAimingCO != null)
            {
                StopCoroutine(_startAimingCO);
                _startAimingCO = null;
            }

            _startAimingCO = StartCoroutine(StartAimingCO());
        }

        private void StopAiming()
        {
            if (!photonView.IsMine)
                return;

            photonView.RPC(nameof(RPC_StartAiming), RpcTarget.All, false);

            if (_startAimingCO != null)
            {
                StopCoroutine(_startAimingCO);
                _startAimingCO = null;
            }

            _isAiming = false;
        }

        private IEnumerator StartAimingCO() //This coroutine is to prevent firing before the camera switching is complete and causing raycast to lend on the player itself
        {
            yield return WaitHandler.GetWaitForSeconds(0.1f);
            _isAiming = true;
        }

        [PunRPC]
        private void RPC_StartAiming(bool active)
        {
            if (_playerGun != null)
                _playerGun?.SetActive(active);

            if (_aimingArmRig != null)
            {
                _aimingArmRig.weight = active ? 1 : 0;
            }
        }

        private void StartShooting()
        {
            _isShooting = true;
        }

        private void StopShooting()
        {
            _isShooting = false;
        }

        private void HandleGunRotation()
        {
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out _hit, Mathf.Infinity))
            {
                _bulletTarget = _hit.point;
            }
            else
            {
                _bulletTarget = _playerCamera.transform.position + _playerCamera.transform.forward * 100f;
            }

            Vector3 gunRotation = _ikTargetTransform.rotation.eulerAngles;
            gunRotation.x = _playerCamera.transform.eulerAngles.x;
            _ikTargetTransform.rotation = Quaternion.Euler(gunRotation);
        }

        private void HandleShooting()
        {
            if (_fireTimer < _fireCD)
            {
                _fireTimer += Time.deltaTime;
            }
            else if (_isAiming && _isShooting)
            {
                SpawnBullet();
                _fireTimer = 0f;
            }
        }

        private void SpawnBullet()
        {
            PhotonNetwork.Instantiate(_bulletPrefab.name, _fireTransform.position, Quaternion.LookRotation(_bulletTarget - _fireTransform.position));
        }
    }
}