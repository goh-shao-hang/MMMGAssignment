using Cinemachine;
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
        [SerializeField] private ThirdPersonMovement _playerThirdPersonMovement;
        [SerializeField] private ThirdPersonCamera _playerThirdPersonCamera;
        [SerializeField] private CinemachineVirtualCamera _playerAimingCamera;
        [SerializeField] private TwoBoneIKConstraint _aimingArmRig;
        [SerializeField] private Transform _ikTargetTransform;
        [SerializeField] private Canvas _crossHairCanvas;
        [SerializeField] ParticleSystem inkParticle;

        [Header("Settings")]
        [SerializeField] private float _fireCD = 0.5f;
        [SerializeField] private LayerMask _targetableLayers; //Purely for increasing accuracy of raycast

        [SerializeField] private bool _hasGun;

        private float _fireTimer = 0f;
        private bool _isAiming = false;
        private bool _isShooting = false;
        private Vector3 _bulletTarget;
        private RaycastHit _hit;
        private Coroutine _startAimingCO;

        private void Start()
        {
            if (_crossHairCanvas != null)
                _crossHairCanvas.gameObject.SetActive(false);
            
            _playerAimingCamera.m_Follow = _playerThirdPersonCamera.CameraFollowTarget;

            //TODO GUN VERY IMPORTANT BLABLA
            EquipGun(true);

            if (_aimingArmRig != null)
            {
                _aimingArmRig.weight = 0;
            }

            _playerAimingCamera.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _playerInputHandler.HoldAimStarted += () => ToggleAiming(true);
            _playerInputHandler.HoldAimEnded += () => ToggleAiming(false);
            _playerInputHandler.ToggleAimInput += () => ToggleAiming(!_isAiming);
            _playerInputHandler.FireInputPressed += () => StartShooting(true);
            _playerInputHandler.FireInputReleased += () => StartShooting(false);
        }

        private void OnDisable()
        {
            _playerInputHandler.HoldAimStarted -= () => ToggleAiming(true);
            _playerInputHandler.HoldAimEnded -= () => ToggleAiming(false);
            _playerInputHandler.ToggleAimInput -= () => ToggleAiming(!_isAiming);
            _playerInputHandler.FireInputPressed -= () => StartShooting(true);
            _playerInputHandler.FireInputReleased -= () => StartShooting(false);
        }

        private void Update()
        {
            //TODO DEBUG
//#if UNITY_EDITOR
            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                EquipGun(!_hasGun);
            }
//#endif

            if (!_hasGun)
                return;

            HandleGunRotation();
            HandleShooting();

            //InkParticle
            if (UnityEngine.Input.GetMouseButtonDown(0))
                inkParticle.Play();
             else if (UnityEngine.Input.GetMouseButtonUp(0))
              inkParticle.Stop();
        }

        public void EquipGun(bool equip)
        {
            _hasGun = equip;

            MobileInputManager.GetInstance()?.SetHasGun(_hasGun);
        }

        private void ToggleAiming(bool aiming)
        {
            if (!photonView.IsMine)
                return;

            if (!_hasGun)
                return;

            if (_crossHairCanvas != null)
                _crossHairCanvas.gameObject.SetActive(aiming);

            _playerThirdPersonMovement.SetIsAiming(aiming);
            _playerAimingCamera.gameObject.SetActive(aiming);
            photonView.RPC(nameof(RPC_StartAiming), RpcTarget.All, aiming);

            if (_startAimingCO != null)
            {
                StopCoroutine(_startAimingCO);
                _startAimingCO = null;
            }

            if (aiming)
            {
                _startAimingCO = StartCoroutine(StartAimingCO()); //isAiming will be set to true here with a delay to prevent bug
            }
            else
            {
                _isAiming = false;

                //TODO
                MobileInputManager.GetInstance()?.SetIsAiming(false);
            }
        }

        private IEnumerator StartAimingCO() //This coroutine is to prevent firing before the camera switching is complete and causing raycast to lend on the player itself
        {
            yield return WaitHandler.GetWaitForSeconds(0.1f);
            _isAiming = true;

            //TODO
            MobileInputManager.GetInstance()?.SetIsAiming(true);
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

        private void StartShooting(bool shooting)
        {
            _isShooting = shooting;
        }

        private void HandleGunRotation()
        {
            if (Physics.Raycast(_playerThirdPersonCamera.Camera.transform.position, _playerThirdPersonCamera.Camera.transform.forward, out _hit, Mathf.Infinity, _targetableLayers))
            {
                _bulletTarget = _hit.point;
            }
            else
            {
                _bulletTarget = _playerThirdPersonCamera.Camera.transform.position + _playerThirdPersonCamera.Camera.transform.forward * 100f;
            }

            Vector3 gunRotation = _ikTargetTransform.rotation.eulerAngles;
            gunRotation.x = _playerThirdPersonCamera.Camera.transform.eulerAngles.x;
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
            Bullet bullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _fireTransform.position, Quaternion.LookRotation(_bulletTarget - _fireTransform.position)).GetComponent<Bullet>();
            bullet.SetOwner(photonView.Owner);
        }
    }
}