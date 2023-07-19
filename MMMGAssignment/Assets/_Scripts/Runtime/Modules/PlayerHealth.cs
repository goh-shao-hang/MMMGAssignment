using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PhotonView _playerPhotonView;
    [SerializeField] private int _maxHealth = 100;

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
        Debug.Log(_currentHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void InstantKill()
    {
        Die();
    }

    private void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
