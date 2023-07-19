using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class PlayerSetup : MonoBehaviourPunCallbacks
    {
        [SerializeField] private ThirdPersonMovement _thirdPersonMovement;
        [SerializeField] private ThirdPersonCamera _thirdPersonCamera;

        public void Awake()
        {   
            _thirdPersonMovement.enabled = photonView.IsMine;
            _thirdPersonCamera.enabled = photonView.IsMine;
        }
    }
}