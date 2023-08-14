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
        [SerializeField] private PlayerHUDManager _playerHUDManager;

        public void Awake()
        {   
            _thirdPersonMovement.enabled = photonView.IsMine;
            _thirdPersonCamera.enabled = photonView.IsMine;
            _playerCamera.gameObject.SetActive(photonView.IsMine);
            _playerHUDManager.gameObject.SetActive(false);
            //TODO HUD
            //_playerHUDManager.gameObject.SetActive(photonView.IsMine);
        }
    }
}