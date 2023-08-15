using GameCells.PhotonNetworking;
using GameCells.Utilities;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Level Data (Required)")]
    [SerializeField] private SO_Level _levelData;

    [Header("Dependencies")]
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private TMP_Text _timerText;

    [Header("Spawn Points")]
    [SerializeField] private float _spawnRadius = 3f;
    [SerializeField] private Transform _team1SpawnPoint;

    [Header("Leave this empty if there is no team.")] 
    [SerializeField] private Transform _team2SpawnPoint;

    [Header("Timers")]
    [SerializeField] private NetworkTimer _countdownTimer;

    [Header("Settings")]
    [SerializeField] private float _countdownDuration = 3f;

    public SO_Level LevelData => _levelData;

    public ELevelState _levelState { get; private set; }

    private GameManager _gameManager;
    private GameManager gameManager => _gameManager ??= GameManager.GetInstance();

    //EVENTS
    public event Action OnLevelPreparing;
    public event Action OnLevelStart;

    private bool _underCountdown = false;

    private void OnEnable()
    {
        gameManager.OnSceneReady += StartCountdown;
        _countdownTimer.OnTimerExpired += StartLevel;
    }

    private void OnDisable()
    {
        gameManager.OnSceneReady -= StartCountdown;
        _countdownTimer.OnTimerExpired -= StartLevel;
    }

    private void StartCountdown()
    {
        _photonView.RPC(nameof(RPC_StartCountdown), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void RPC_StartCountdown()
    {
        SpawnLocalPlayerManager();

        this._levelState = ELevelState.Preparing;
        OnLevelPreparing?.Invoke();
        Debug.Log("Level Preparing...");


        if (PhotonNetwork.IsMasterClient)
        {
            _countdownTimer.ServerStartTimer(_countdownDuration);
        }

        _underCountdown = true;
        StartCoroutine(StartCountdownCO());
    }

    private void StartLevel()
    {
        _levelState = ELevelState.Running;
        _underCountdown = false;

        OnLevelStart?.Invoke();
    }

    private void SpawnLocalPlayerManager()
    {
        PhotonNetwork.Instantiate(gameManager.playerManagerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public Vector3 GetSpawnPoint()
    {
        Vector3 spawnPos = _team1SpawnPoint.position + new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0f, Random.Range(-_spawnRadius, _spawnRadius));
        return spawnPos;
    }

    private IEnumerator StartCountdownCO()
    {
        _countdownText.gameObject.SetActive(true);

        while (_underCountdown)
        {
            _countdownText.text = _countdownTimer.CurrentRemainingTime.ToString("n0");

            yield return null;
        }

        _countdownText.gameObject.SetActive(false);
    }
}
