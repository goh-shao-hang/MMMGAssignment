using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCells.Utilities;

public class Bullet : MonoBehaviourPun
{
    [Header("Dependencies")]
    [SerializeField] private Rigidbody _bulletRigidbody;
    [SerializeField] private LayerMask _damageableLayers;
    [SerializeField] private LayerMask _obstacleLayers;
    [SerializeField] private ParticleSystem _trailParticles;
    [SerializeField] private ParticleSystem _hitParticlesPrefab;

    [Header("Settings")]
    [SerializeField] private int _bulletDamage = 20;
    [SerializeField] private float _bulletSpeed = 15f;

    private Vector3 _hitPosition;

    private void OnEnable()
    {
        _bulletRigidbody.velocity = transform.forward * _bulletSpeed; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (Helper.CompareLayer(other.gameObject, _damageableLayers))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(_bulletDamage);
            DestroyBullet();
        }
        else if (Helper.CompareLayer(other.gameObject, _obstacleLayers))
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        if (_trailParticles != null)
        {
            _trailParticles.transform.parent = null;
            _trailParticles.Stop();
        }

        photonView.RPC(nameof(SpawnHitParticles), RpcTarget.All, transform.position);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void SpawnHitParticles(Vector3 spawnPosition)
    {
        if (_hitParticlesPrefab != null)
        {
            GameObject hitParticles = Instantiate(_hitParticlesPrefab, spawnPosition, Quaternion.identity).gameObject;
            Destroy(hitParticles, 1.5f);
        }
    }
}
