using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class PlayerManager : MonoBehaviourPun
    {
        [Header("Dependencies")]
        [SerializeField] private GameObject _playerControllerPrefab;

        //EVENTS
        public event Action<string> OnUsernameChanged;
        public event Action<float> OnHealthChanged;

        private GameObject PlayerObject = null;

        private LevelManager _levelManager;
        private LevelManager levelManager => _levelManager ??= LevelManager.GetInstance();

        private void Start()
        {
            if (photonView.IsMine)
            {
                UpdateUsername();
                SpawnPlayerController();
            }
        }

        private void UpdateUsername()
        {
            OnUsernameChanged?.Invoke(photonView.Owner.NickName);
        }

        private void SpawnPlayerController()
        {
            if (levelManager != null)
            {
                PlayerObject = PhotonNetwork.Instantiate(_playerControllerPrefab.name, levelManager.GetRandomSpawnPoint().position, Quaternion.identity);
            }
            else
            {
                //TODO better spawn
                PlayerObject = PhotonNetwork.Instantiate(_playerControllerPrefab.name, FindObjectOfType<NetworkDebugger>().SpawnPoint.position, Quaternion.identity);
            }

            PlayerObject.GetComponent<PlayerHealth>().Initialize(this);
        }

        public void OnPlayerHealthChanged(float healthPercentage)
        {
            OnHealthChanged?.Invoke(healthPercentage);
        }

        public void OnPlayerDeath()
        {
            PhotonNetwork.Destroy(PlayerObject);
            PlayerObject = null;
        }
    }
}