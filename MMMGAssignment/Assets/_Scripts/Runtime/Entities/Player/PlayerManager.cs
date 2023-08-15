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
        public event Action<float> OnHealthChanged;

        private GameObject PlayerObject = null;

        private LevelManager _levelManager;
        private LevelManager levelManager => _levelManager ??= LevelManager.GetInstance();

        private void Awake()
        {
            if (photonView.IsMine)
            {
                SpawnPlayerController();
            }
        }

        private void OnEnable()
        {
            if (!photonView.IsMine)
                return;

            if (levelManager != null)
            {
                levelManager.OnLevelPreparing += LockPlayerInput;
                levelManager.OnLevelStart += UnlockPlayerInput;
            }
        }

        private void OnDisable()
        {
            if (!photonView.IsMine)
                return;

            if (levelManager != null)
            {
                levelManager.OnLevelPreparing -= LockPlayerInput;
                levelManager.OnLevelStart -= UnlockPlayerInput;
            }
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

        public void LockPlayerInput()
        {
            PlayerObject.GetComponent<PlayerInputHandler>().LockInput(true);
        }

        public void UnlockPlayerInput()
        {
            PlayerObject.GetComponent<PlayerInputHandler>().LockInput(false);
        }
    }
}