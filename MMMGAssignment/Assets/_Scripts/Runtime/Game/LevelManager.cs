using GameCells.Utilities;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Dependencies")]
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private SO_Level _levelData;
    [SerializeField] private Transform[] _team1SpawnPoints;
    [Tooltip("Leave this empty if there is no team.")] 
    [SerializeField] private Transform[] _team2SpawnPoints;

    public ELevelState _levelState { get; private set; }

    private GameManager _gameManager;
    private GameManager gameManager => _gameManager ??= GameManager.GetInstance();

    private void Start()
    {
        this._levelState = ELevelState.Preparing;
        Debug.Log("Level Preparing...");
    }

    private void OnEnable()
    {
        //TODO start countdown instead?
        gameManager.OnSceneReady += StartCountdown;
        CountdownTimer.OnCountdownTimerHasExpired += StartLevel;
    }

    private void OnDisable()
    {
        gameManager.OnSceneReady -= StartCountdown;
        CountdownTimer.OnCountdownTimerHasExpired -= StartLevel;
    }

    private void Update()
    {
        //TODO start debug
        /*if (Input.GetKeyDown(KeyCode.S))
        {
            StartCountdown();
        }*/
        
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
            CountdownTimer.SetStartTime();
        }
    }

    private void StartLevel()
    {
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
}
