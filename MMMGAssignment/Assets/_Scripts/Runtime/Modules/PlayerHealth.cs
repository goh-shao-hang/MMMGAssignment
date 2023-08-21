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

        public event Action OnTakeDamage;

        public PhotonView PlayerPhotonView => _playerPhotonView;
        public PlayerManager PlayerManager { get; private set; }

        private LevelManager _levelManager;
        private LevelManager levelManager => _levelManager ??= LevelManager.GetInstance();

        public const int MAX_HEALTH = 100;
        private int _currentHealth;

        public PlayerHealth Initialize(PlayerManager playerManager)
        {
            this.PlayerManager = playerManager;
            return this;
        }

        private void Start()
        {
            _currentHealth = MAX_HEALTH;
            PlayerManager?.OnPlayerHealthChanged(1);
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
            OnTakeDamage?.Invoke();
            _playerPhotonView.RPC(nameof(RPC_TakeDamage), _playerPhotonView.Owner, damage);
        }

        public void TakeDamage(int damage, Photon.Realtime.Player damager)
        {
            if (levelManager == null) //Probably offline, only consider the damage
            {
                TakeDamage(damage);
            }
            else
            {
                if (!levelManager.LevelData.HasTeam) //No team, just damage
                {
                    TakeDamage(damage);
                }
                else
                {
                    //Team is same, return
                    if ((int)damager.CustomProperties[GameData.TEAM_INFO_HASH] == (int)_playerPhotonView.Owner.CustomProperties[GameData.TEAM_INFO_HASH])
                    {
                        Debug.LogError("Bullet collided but this is ur fren");
                        return;
                    }

                    TakeDamage(damage);
                }
            }
        }

        [PunRPC]
        private void RPC_TakeDamage(int damage)
        {
            _currentHealth -= damage;

            PlayerManager?.OnPlayerHealthChanged((float)_currentHealth / (float)MAX_HEALTH);

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

            PlayerManager?.OnPlayerHealthChanged((float)_currentHealth / (float)MAX_HEALTH);

            Die();
        }

        public void Die()
        {
            _playerPhotonView.RPC(nameof(RPC_SpawnDeathParticles), RpcTarget.All);

            if (PlayerManager != null)
            {
                PlayerManager?.OnPlayerDeath();
            }
            else
            {
                Destroy(gameObject);
            }
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