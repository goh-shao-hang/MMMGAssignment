using GameCells.PhotonNetworking;
using GameCells.Utilities;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Dependencies")]
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private SO_Level _levelData;
    [SerializeField] private Transform[] _team1SpawnPoints;
    [Tooltip("Leave this empty if there is no team.")] 
    [SerializeField] private Transform[] _team2SpawnPoints;

    [Header("Settings")]
    [SerializeField] private float _countdownDuration = 3f;

    //TODO TIMER
    [Space]
    [SerializeField] private NetworkTimer _levelTimer;

    public ELevelState _levelState { get; private set; }

    private GameManager _gameManager;
    private GameManager gameManager => _gameManager ??= GameManager.GetInstance();

    private bool _underCountdown = false;

    private void Start()
    {
        this._levelState = ELevelState.Preparing;
        Debug.Log("Level Preparing...");
    }

    private void OnEnable()
    {
        //TODO start countdown instead?
        gameManager.OnSceneReady += StartCountdown;
        _levelTimer.OnTimerExpired += StartLevel;
    }

    private void OnDisable()
    {
        gameManager.OnSceneReady -= StartCountdown;
        _levelTimer.OnTimerExpired -= StartLevel;

    }

    private void StartCountdown()
    {
        _photonView.RPC(nameof(RPC_StartCountdown), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void RPC_StartCountdown()
    {
        Debug.Log("Countdown");
        _levelState = ELevelState.Running;

        if (PhotonNetwork.IsMasterClient)
        {
            _levelTimer.ServerStartTimer(_countdownDuration);
        }

        _underCountdown = true;
        StartCoroutine(StartCountdownCO());
    }

    private void StartLevel()
    {
        _underCountdown = false;

        SpawnLocalPlayerManager();
    }

    private void SpawnLocalPlayerManager()
    {
        PhotonNetwork.Instantiate(gameManager.playerManagerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public Transform GetRandomSpawnPoint()
    {
        return _team1SpawnPoints[Random.Range(0, _team1SpawnPoints.Length)];
    }

    private IEnumerator StartCountdownCO()
    {
        _countdownText.gameObject.SetActive(true);

        while (_underCountdown)
        {
            _countdownText.text = _levelTimer.CurrentRemainingTime.ToString("n0");

            yield return null;
        }

        _countdownText.gameObject.SetActive(false);
    }
}
