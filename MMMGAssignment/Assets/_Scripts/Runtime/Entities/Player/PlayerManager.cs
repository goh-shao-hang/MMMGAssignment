using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
{
    [SerializeField] private GameObject _playerControllerPrefab;

    private LevelManager _levelManager;
    private LevelManager levelManager => _levelManager ??= LevelManager.GetInstance();

    private void Awake()
    {
        if (photonView.IsMine)
        {
            SpawnPlayerController();
        }
    }

    private void SpawnPlayerController()
    {
        Debug.Log("spawned");
        PhotonNetwork.Instantiate(_playerControllerPrefab.name, levelManager.GetRandomSpawnPoint().position, Quaternion.identity);
    }
}
