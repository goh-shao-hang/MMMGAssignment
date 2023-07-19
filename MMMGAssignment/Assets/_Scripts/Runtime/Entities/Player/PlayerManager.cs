using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _playerPrefab;

    private PhotonView _photonView;
}
