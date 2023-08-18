using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameCells.Player
{
    public class PlayerSetup : MonoBehaviourPun
    {
        [SerializeField] private ThirdPersonMovement _thirdPersonMovement;
        [SerializeField] private ThirdPersonCamera _thirdPersonCamera;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private TMP_Text _usernameText;

        private Camera _billboardTargetCamera;

        public void Start()
        {
            //TODO
            _billboardTargetCamera = GameObject.FindAnyObjectByType<Camera>();

            bool isMine = photonView.IsMine;
            _thirdPersonMovement.enabled = isMine;
            _thirdPersonCamera.enabled = isMine;
            _playerCamera.gameObject.SetActive(isMine);
            _usernameText.gameObject.SetActive(!isMine);
            _usernameText.SetText(photonView.Owner.NickName);
        }

        private void LateUpdate()
        {
            if (_billboardTargetCamera == null)
                return;

            _usernameText.transform.LookAt(_billboardTargetCamera.transform);
            _usernameText.transform.Rotate(Vector3.up * 180f);
        }

        public void Initialize(PlayerManager playerManager)
        {
            this._billboardTargetCamera = playerManager.PlayerController.GetComponent<ThirdPersonCamera>().Camera;
        }
    }
}