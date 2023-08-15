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
    [SerializeField] private TMP_Text _levelTimerText;
    [SerializeField] private TMP_Text _roundEndText;

    [Header("Spawn Points")]
    [SerializeField] private float _spawnRadius = 3f;
    [SerializeField] private Transform _team1SpawnPoint;

    [Header("Leave this empty if there is no team.")] 
    [SerializeField] private Transform _team2SpawnPoint;

    [Header("Timers")]
    [SerializeField] private NetworkTimer _countdownTimer;
    [SerializeField] private NetworkTimer _levelTimer;

    [Header("Settings")]
    [SerializeField] private float _countdownDuration = 3f;

    public SO_Level LevelData => _levelData;

    public ELevelState _levelState { get; private set; } = ELevelState.Preparing;

    private GameManager _gameManager;
    private GameManager gameManager => _gameManager ??= GameManager.GetInstance();

    //EVENTS
    public event Action OnLevelCountdown;
    public event Action OnLevelStart;
    public event Action OnLevelEnd;

    private void Awake()
    {
        _levelState = ELevelState.Preparing;

        _roundEndText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //Start countdown when all players are ready
        gameManager.OnAllPlayersJoinedScene += StartCountdown;

        _countdownTimer.OnTimerExpired += StartLevel;
        _levelTimer.OnTimerExpired += EndLevel;
    }

    private void OnDisable()
    {
        gameManager.OnAllPlayersJoinedScene -= StartCountdown;

        _countdownTimer.OnTimerExpired -= StartLevel;
        _levelTimer.OnTimerExpired -= EndLevel;
    }

    private void StartCountdown()
    {
        _photonView.RPC(nameof(RPC_StartCountdown), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void RPC_StartCountdown()
    {
        SpawnLocalPlayerManager();

        this._levelState = ELevelState.Countdown;

        OnLevelCountdown?.Invoke();
        Debug.Log("LEVEL COUNTDOWN");

        if (PhotonNetwork.IsMasterClient)
        {
            _countdownTimer.ServerStartTimer(_countdownDuration);
        }

        StartCoroutine(StartCountdownCO());
    }

    private void StartLevel()
    {
        this._levelState = ELevelState.Running;
        OnLevelStart?.Invoke();

        if (PhotonNetwork.IsMasterClient)
        {
            _levelTimer.ServerStartTimer(LevelData.RoundDuration);
        }

        StartCoroutine(LevelTimerCO());
    }

    private void EndLevel()
    {
        this._levelState = ELevelState.Ended;
        OnLevelEnd?.Invoke();

        _roundEndText.gameObject.SetActive(true);
        //Time.timeScale = 0f;

        //Tell the game manager that the current level has ended and can go to the next one
        gameManager.EndCurrentLevel();

        //TODO
        Destroy(gameObject);
    }

    private void SpawnLocalPlayerManager()
    {
        Debug.LogError("once");
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

        while (_levelState == ELevelState.Countdown)
        {
            _countdownText.text = _countdownTimer.CurrentRemainingTime.ToString("n0");

            yield return null;
        }

        _countdownText.gameObject.SetActive(false);
    }

    private IEnumerator LevelTimerCO()
    {
        _levelTimerText.gameObject.SetActive(true);

        while (_levelState == ELevelState.Running)
        {
            _levelTimerText.text = _levelTimer.CurrentRemainingTime.ToString("n0");

            yield return null;
        }

        _levelTimerText.gameObject.SetActive(false);
    }
}
