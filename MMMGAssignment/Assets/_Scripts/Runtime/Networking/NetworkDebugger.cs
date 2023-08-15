using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using GameCells.Player;

public class NetworkDebugger : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool _startInOfflineMode = false;
    [SerializeField] private bool _spawnPlayer = false;

    [Space]

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _spawnPoint;

    public Transform SpawnPoint => _spawnPoint;

    void Start()
    {
        if (!_startInOfflineMode)
        {
            Debug.Log("Connecting...");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.OfflineMode = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to server.");

        if (!PhotonNetwork.OfflineMode)
            PhotonNetwork.JoinLobby();
        else
            PhotonNetwork.JoinOrCreateRoom("test", null, null); //If offline, no need to join lobby
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("In lobby now");

        PhotonNetwork.JoinOrCreateRoom("test", null, null);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("Welcome to the room boi");

        if (_spawnPlayer)
        {
            GameObject newPlayer = PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPoint.position, Quaternion.identity);
        }
    }
}
