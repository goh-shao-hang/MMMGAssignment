using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class PlayerSetup : MonoBehaviourPun
    {
        [SerializeField] private ThirdPersonMovement _thirdPersonMovement;
        [SerializeField] private ThirdPersonCamera _thirdPersonCamera;
        [SerializeField] private Camera _playerCamera;

        public void Awake()
        {
            bool isMine = photonView.IsMine;
            _thirdPersonMovement.enabled = isMine;
            _thirdPersonCamera.enabled = isMine;
            _playerCamera.gameObject.SetActive(isMine);
        }
    }
}