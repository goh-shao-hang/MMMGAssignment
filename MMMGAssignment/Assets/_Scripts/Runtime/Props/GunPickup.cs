using GameCells.Player;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviourPun
{
    [SerializeField] private GameObject _pickedUpParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerShooting playerShooting))
        {
            if (_pickedUpParticles != null)
            {
                Instantiate(_pickedUpParticles, transform.position, Quaternion.identity);
            }

            playerShooting.EquipGun(true);
            photonView.RPC(nameof(RPC_PickedUp), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_PickedUp()
    {
        Destroy(gameObject);
    }
}
