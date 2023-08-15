using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class PlayerManager : MonoBehaviourPun
    {
        //Player Manager is responsible for communicating between players without relying on the existance of the player controller

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
                Debug.LogError($"Spawned {photonView.Owner.NickName}");
            }
        }

        private void OnEnable()
        {
            if (!photonView.IsMine)
                return;

            if (levelManager != null)
            {
                levelManager.OnLevelCountdown += LockPlayerInput;
                levelManager.OnLevelStart += UnlockPlayerInput;
                levelManager.OnLevelEnd += DestroyPlayer;
            }
        }

        private void OnDisable()
        {
            if (!photonView.IsMine)
                return;

            if (levelManager != null)
            {
                levelManager.OnLevelCountdown -= LockPlayerInput;
                levelManager.OnLevelStart -= UnlockPlayerInput;
                levelManager.OnLevelEnd -= DestroyPlayer;
            }
        }

        private void SpawnPlayerController()
        {
            if (levelManager != null)
            {
                PlayerObject = PhotonNetwork.Instantiate(_playerControllerPrefab.name, levelManager.GetSpawnPoint(), Quaternion.identity);
            }
            else
            {
                //TODO better spawn
                PlayerObject = PhotonNetwork.Instantiate(_playerControllerPrefab.name, FindObjectOfType<NetworkDebugger>().SpawnPoint.position, Quaternion.identity);
            }

            //Initialize Health
            PlayerObject.GetComponent<PlayerHealth>().Initialize(this);

            //Initialize Gun
            PlayerObject.GetComponent<PlayerShooting>().EquipGun(levelManager.LevelData.StartWithGun);
        }

        public void OnPlayerHealthChanged(float healthPercentage)
        {
            OnHealthChanged?.Invoke(healthPercentage);
        }

        public void OnPlayerDeath()
        {
            DestroyPlayer();
        }

        public void DestroyPlayer()
        {
            PhotonNetwork.Destroy(PlayerObject);
            PlayerObject = null;
            Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            Debug.LogError($"{photonView.Owner.NickName} destroyed");
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