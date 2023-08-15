using GameCells.PhotonNetworking;
using GameCells.Utilities;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Dependencies")]
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private SO_Level _levelData;
    [SerializeField] private Transform[] _team1SpawnPoints;
    [Tooltip("Leave this empty if there is no team.")] 
    [SerializeField] private Transform[] _team2SpawnPoints;

    [Header("Timers")]
    [SerializeField] private NetworkTimer _countdownTimer;

    [Header("Settings")]
    [SerializeField] private float _countdownDuration = 3f;

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

    public Transform GetRandomSpawnPoint()
    {
        return _team1SpawnPoints[UnityEngine.Random.Range(0, _team1SpawnPoints.Length)];
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
