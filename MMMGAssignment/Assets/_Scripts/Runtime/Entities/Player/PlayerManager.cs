using GameCells.Utilities;
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
        public event Action OnPlayerEliminated;
        public event Action OnPlayerWon;
        public event Action OnPlayerRespawnStart;
        public event Action<float> OnPlayerRespawnTimeUpdate;
        public event Action OnPlayerRespawnEnd;

        public GameObject PlayerController { get; private set; } = null;

        private LevelManager _levelManager;
        private LevelManager levelManager => _levelManager ??= LevelManager.GetInstance();


        public float CurrentRespawningTime { get; private set; }

        public int TeamNumber { get; private set; }

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
                levelManager.OnLevelCountdown += LockPlayerInput;
                levelManager.OnLevelStart += UnlockPlayerInput;
                levelManager.OnLevelEnd += LockPlayerInput;
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
                levelManager.OnLevelEnd -= LockPlayerInput;
            }
        }

        private void SpawnPlayerController()
        {
            if (levelManager != null)
            {
                PlayerController = PhotonNetwork.Instantiate(_playerControllerPrefab.name, levelManager.GetTeam1SpawnPoint(), Quaternion.identity);
            }
            else
            {
                //TODO better spawn
                PlayerController = PhotonNetwork.Instantiate(_playerControllerPrefab.name, FindObjectOfType<NetworkDebugger>().SpawnPoint.position, Quaternion.identity);
            }

            //InitializeHealth
            PlayerController.GetComponent<PlayerHealth>().Initialize(this);

            //Initialize Gun
            PlayerController.GetComponent<PlayerShooting>().EquipGun(levelManager.LevelData.StartWithGun);

            //Initialize Team
            if (levelManager.LevelData.HasTeam)
            {
                TeamNumber = (int)(PhotonNetwork.LocalPlayer.CustomProperties[GameData.TEAM_INFO_HASH]);
                PlayerController.GetComponentInChildren<PlayerColorManager>().OverrideColor(TeamNumber);
                Debug.Log(TeamNumber);

                //Move to spawn point
                if (TeamNumber == 2)
                {
                    PlayerController.transform.position = levelManager.GetTeam2SpawnPoint();
                }
            }
        }

        public void OnPlayerHealthChanged(float healthPercentage)
        {
            OnHealthChanged?.Invoke(healthPercentage);
        }

        public void OnPlayerDeath()
        {
            DestroyPlayerController();

            if (levelManager.LevelData.HasTeam)
            {
                //Team number has been set when spawning
                levelManager.OnTeammateKilled(this.TeamNumber);
            }

            if (levelManager.LevelData.CanRespawn)
            {
                StartCoroutine(PlayerRespawnCO());
            }
            else
            {
                EliminatePlayer();
            }
        }

        //Player loses this round and cannot respawn
        public void EliminatePlayer()
        {
            OnPlayerEliminated?.Invoke();
        }

        //Player wins and gets removed to wait for next round
        public void PlayerWinRound()
        {
            DestroyPlayerController();
            OnPlayerWon?.Invoke();
            photonView.RPC(nameof(RPC_PlayerWon), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void RPC_PlayerWon()
        {
            levelManager.ServerLevelEnd();
        }


        public void DestroyPlayerController()
        {
            PhotonNetwork.Destroy(PlayerController);
            PlayerController = null;
        }

        private IEnumerator PlayerRespawnCO()
        {
            CurrentRespawningTime = 0f;
            OnPlayerRespawnStart?.Invoke();

            while (CurrentRespawningTime < GameData.RESPAWN_TIME)
            {
                CurrentRespawningTime += Time.deltaTime;

                OnPlayerRespawnTimeUpdate?.Invoke(CurrentRespawningTime);

                if (levelManager.CurrentLevelState != ELevelState.Running) //If game isn't running anymore, exit coroutine
                {
                    OnPlayerRespawnEnd?.Invoke();
                    yield break;
                }

                yield return null;
            }

            OnPlayerRespawnEnd?.Invoke();

            if (levelManager.CurrentLevelState == ELevelState.Running)
                SpawnPlayerController();
        }

        public void LockPlayerInput()
        {
            PlayerController.GetComponent<PlayerInputHandler>().LockInput(true);
        }

        public void UnlockPlayerInput()
        {
            PlayerController.GetComponent<PlayerInputHandler>().LockInput(false);
        }
    }
}