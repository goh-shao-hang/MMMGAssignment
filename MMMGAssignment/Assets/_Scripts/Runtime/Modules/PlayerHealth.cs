using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

namespace GameCells.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private PhotonView _playerPhotonView;
        [SerializeField] private GameObject _deathParticles;

        //TODO
        //public event Action<float> OnHealthChanged;

        public PhotonView PlayerPhotonView => _playerPhotonView;
        private PlayerManager _playerManager;

        public const int MAX_HEALTH = 100;
        private int _currentHealth;

        public PlayerHealth Initialize(PlayerManager playerManager)
        {
            this._playerManager = playerManager;
            return this;
        }

        private void Start()
        {
            _currentHealth = MAX_HEALTH;
            _playerManager?.OnPlayerHealthChanged(1);
        }

        private void Update()
        {
            if (!_playerPhotonView.IsMine)
                return;

            //TODO debug damage
#if UNITY_EDITOR
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                TakeDamage(50);
            }
#endif
        }

        public void TakeDamage(int damage)
        {
            _playerPhotonView.RPC(nameof(RPC_TakeDamage), _playerPhotonView.Owner, damage);
        }

        [PunRPC]
        private void RPC_TakeDamage(int damage)
        {
            _currentHealth -= damage;

            _playerManager?.OnPlayerHealthChanged((float)_currentHealth / (float)MAX_HEALTH);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void InstantKill()
        {
            _playerPhotonView.RPC(nameof(RPC_InstantKill), _playerPhotonView.Owner);
        }

        [PunRPC]
        public void RPC_InstantKill()
        {
            _currentHealth = 0;

            _playerManager?.OnPlayerHealthChanged((float)_currentHealth / (float)MAX_HEALTH);

            Die();
        }

        public void Die()
        {
            _playerPhotonView.RPC(nameof(RPC_SpawnDeathParticles), RpcTarget.All);
            _playerManager?.OnPlayerDeath();
        }

        [PunRPC]
        private void RPC_SpawnDeathParticles()
        {
            if (_deathParticles != null)
            {
                //TODO: particle in seperate script
                GameObject particles = Instantiate(_deathParticles, transform.position + Vector3.up, Quaternion.identity);
                Destroy(particles, 3f);
            }
        }
    }
}