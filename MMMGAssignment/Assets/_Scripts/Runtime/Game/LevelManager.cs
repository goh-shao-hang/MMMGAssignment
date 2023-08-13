using GameCells.Utilities;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Dependencies")]
    [SerializeField] private SO_Level _levelData;
    [SerializeField] private Transform[] _spawnPoints;

    public ELevelState _levelState { get; private set; }

    private GameManager _gameManager;
    private GameManager gameManager => _gameManager ??= GameManager.GetInstance();

    private void Start()
    {
        this._levelState = ELevelState.Preparing;
    }

    private void OnEnable()
    {
        gameManager.OnLevelReady += StartLevel;
    }

    private void OnDisable()
    {
        gameManager.OnLevelReady -= StartLevel;
    }

    private void StartLevel()
    {
        SpawnLocalPlayer();
    }

    private void SpawnLocalPlayer()
    {
        PhotonNetwork.Instantiate(gameManager.playerManagerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public Transform GetRandomSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Length)];
    }
}
