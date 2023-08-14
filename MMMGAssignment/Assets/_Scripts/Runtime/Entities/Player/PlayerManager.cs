using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class PlayerManager : MonoBehaviourPun
    {
        [Header("Dependencies")]
        [SerializeField] private GameObject _playerControllerPrefab;
        [SerializeField] private PlayerHUDManager _playerHUDManager;
        [SerializeField] private GameObject _deathParticles;

        private LevelManager _levelManager;
        private LevelManager levelManager => _levelManager ??= LevelManager.GetInstance();

        private PlayerHealth _playerHealth;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                SpawnPlayerController();
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void SpawnPlayerController()
        {
            GameObject playerObject;

            if (levelManager != null)
            {
                playerObject = PhotonNetwork.Instantiate(_playerControllerPrefab.name, levelManager.GetRandomSpawnPoint().position, Quaternion.identity);
            }
            else
            {
                //TODO better spawn
                playerObject = PhotonNetwork.Instantiate(_playerControllerPrefab.name, FindObjectOfType<NetworkDebugger>().SpawnPoint.position, Quaternion.identity);
            }

            _playerHealth = playerObject.GetComponent<PlayerHealth>();
        }

        private void OnPlayerHealthChanged()
        {
            /*//Update UI
            _playerHUDManager.UpdateHealthUI(_playerHealth.CurrentHealth / _playerHealth.MaxHealth);

            //Handle death
            if (_playerHealth.CurrentHealth <= 0)
            {
                Die();
            }*/
        }

        public void Die()
        {
            photonView.RPC(nameof(RPC_Die), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_Die()
        {
            if (_deathParticles != null)
            {
                //TODO: particle in seperate script
                GameObject particles = Instantiate(_deathParticles, transform.position + Vector3.up, Quaternion.identity);
                Destroy(particles, 3f);
            }

            Destroy(gameObject);
        }
    }
}