using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCells.Utilities;
using Photon.Realtime;

public class Bullet : MonoBehaviourPun
{
    [Header("Dependencies")]
    [SerializeField] private Rigidbody _bulletRigidbody;
    [SerializeField] private LayerMask _damageableLayers;
    [SerializeField] private LayerMask _obstacleLayers;
    [SerializeField] private ParticleSystem _trailParticles;
    [SerializeField] private ParticleSystem _hitParticlesPrefab;
    [SerializeField] private GameObject decal;

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

                // Get the hit normal at the collision point
                Vector3 hitNormal = other.ClosestPoint(transform.position) - transform.position;
                
                // Adjust the hit normal to make sure it's pointing outward from the surface
                hitNormal = AdjustHitNormal(hitNormal);

                // Get the bullet direction (using _bulletRigidbody.velocity.normalized)
                Vector3 bulletDirection = _bulletRigidbody.velocity.normalized;

                // Place the decal with the adjusted rotation
                PlaceDecal(transform.position, hitNormal, bulletDirection);

                DestroyBullet();
            }
        }
        else if (Helper.CompareLayer(other.gameObject, _obstacleLayers))
        {
            Vector3 spawnPosition = transform.position;

            // Cast a ray from spawnPosition downwards to hit the surface and get the hit normal
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up, Vector3.down, out hit))
            {
                Vector3 hitNormal = hit.normal;
                
                // Adjust the hit normal to make sure it's pointing outward from the surface
                hitNormal = AdjustHitNormal(hitNormal);

                // Get the bullet direction (using _bulletRigidbody.velocity.normalized)
                Vector3 bulletDirection = _bulletRigidbody.velocity.normalized;

                // Place the decal with the adjusted rotation
                PlaceDecal(spawnPosition, hitNormal, bulletDirection);
            }

            DestroyBullet();
        }
    }

    private void PlaceDecal(Vector3 spawnPosition, Vector3 hitNormal, Vector3 bulletDirection)
    {
        // Create and position the decal
        GameObject decalObject = Instantiate(decal, spawnPosition, Quaternion.identity);

        // Determine which rotation method to use based on hitNormal
        Quaternion rotation;
        if (IsVerticalSurface(hitNormal))
        {
            // Use bulletDirection and up vector to calculate rotation for vertical surfaces
            rotation = Quaternion.LookRotation(Vector3.Cross(hitNormal, bulletDirection), hitNormal);
        }
        else
        {
            // Use Quaternion.LookRotation for horizontal surfaces
            rotation = Quaternion.LookRotation(hitNormal);
        }

        // Apply the rotation to the decal
        decalObject.transform.rotation = rotation;

        Destroy(decalObject, 5f); // Destroy the decal after a certain time
    }


    private bool IsVerticalSurface(Vector3 normal)
    {
        // Define a threshold angle to determine if the surface is vertical
        float thresholdAngle = 180f; // You can adjust this value

        // Calculate the angle between the normal and the up direction
        float angle = Vector3.Angle(normal, Vector3.up);

        // Return true if the angle is greater than the threshold
        return angle > thresholdAngle;
    }
    private Vector3 AdjustHitNormal(Vector3 hitNormal)
    {
        // Check if the hit normal is pointing downward (possible floor hit)
        if (hitNormal.y < 0)
        {
            // Invert the hit normal to point outward
            hitNormal = -hitNormal;
        }

        return hitNormal;
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
