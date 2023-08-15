using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCells.Utilities;
using Photon.Realtime;
using GameCells.Player;

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
    [SerializeField] private float _maxBulletLifetime = 5f;

    private Vector3 _hitPosition;
    private Coroutine _selfDestructCO;

    private Player _owner;

    private void OnEnable()
    {
        _bulletRigidbody.velocity = transform.forward * _bulletSpeed;

        if (_selfDestructCO != null)
        {
            StopCoroutine(_selfDestructCO);
            _selfDestructCO = null;
        }

        _selfDestructCO = StartCoroutine(SelfDestructCO());
    }

    public void SetOwner(Player owner)
    {
        _owner = owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (Helper.CompareLayer(other.gameObject, _damageableLayers))
        {
            if (other.TryGetComponent(out PlayerHealth playerHealth))
            {
                if (playerHealth.PlayerPhotonView.Owner == _owner)
                    return;

                other.GetComponent<PlayerHealth>()?.TakeDamage(_bulletDamage);
                DestroyBullet();
            }
        }
        else if (Helper.CompareLayer(other.gameObject, _obstacleLayers))
        {
            DestroyBullet();
        }
    }

    private IEnumerator SelfDestructCO()
    {
        yield return WaitHandler.GetWaitForSeconds(_maxBulletLifetime);
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if (_selfDestructCO != null)
        {
            StopCoroutine(_selfDestructCO);
            _selfDestructCO = null;
        }

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
