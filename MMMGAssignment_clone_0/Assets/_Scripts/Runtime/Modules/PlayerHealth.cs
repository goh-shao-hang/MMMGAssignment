using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PhotonView _playerPhotonView;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private GameObject _deathParticles;

    private int _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        if (!_playerPhotonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(50);
        }

    }

    public void TakeDamage(int damage)
    {
        _playerPhotonView.RPC(nameof(RPC_TakeDamage), _playerPhotonView.Owner, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void InstantKill()
    {
        Die();
    }

    public void Die()
    {
        _playerPhotonView.RPC(nameof(RPC_Die), RpcTarget.All);
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