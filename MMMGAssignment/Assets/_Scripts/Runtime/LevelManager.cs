using GameCells;
using GameCells.PhotonNetworking;
using GameCells.Utilities;
using Photon.Pun;
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
    public SO_Level LevelData => _levelData;

    [Header("Photon View")]
    [SerializeField] private PhotonView _photonView;

    [Header("Spawn Points")]
    [SerializeField] private float _spawnRadius = 2f;
    [SerializeField] private Transform _team1SpawnPoint;
    [Header("Leave empty if playing as individuals")]
    [SerializeField] private Transform _team2SpawnPoint;

    [Header("Timers")]
    [SerializeField] private BetterNetworkTimer _countdownTimer;
    [SerializeField] private BetterNetworkTimer _levelTimer;

    [Header("UI")]
    [SerializeField] private TMP_Text _countdownTimerText; 
    [SerializeField] private TMP_Text _levelTimerText;
    [SerializeField] private TMP_Text _roundEndText;

    private GameManager _gameManager;

    private ELevelState _currentLevelState;

    //EVENTS
    public event Action OnLevelPreparing;
    public event Action OnLevelCountdown;
    public event Action OnLevelStart;
    public event Action OnLevelEnd;

    private void Awake()
    {
        //Called on everyone
        _currentLevelState = ELevelState.Preparing;

        _countdownTimerText.gameObject.SetActive(false);
        _levelTimerText.gameObject.SetActive(false);

        _roundEndText.gameObject.SetActive(false);

        //Called on master client only
        if (!PhotonNetwork.IsMasterClient)
            return;

        _gameManager = GameManager.GetInstance();
    }

    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        _gameManager.OnLevelFinishLoading += ServerLevelCountdown;

        //Subscribe to timers if is masterClient
        _countdownTimer.OnTimerExpired += ServerLevelStart;
        _levelTimer.OnTimerExpired += ServerLevelEnd;
    }

    private void OnDisable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        _gameManager.OnLevelFinishLoading -= ServerLevelCountdown;

        //Unsubscribe timers if is masterClient
        _countdownTimer.OnTimerExpired -= ServerLevelStart;
        _levelTimer.OnTimerExpired -= ServerLevelEnd;
    }

    public void ServerLevelCountdown()
    {
        _countdownTimer.StartTimerAsServer(GameData.LEVEL_COUNTDOWN_TIME);
        _photonView.RPC(nameof(RPC_LevelCountdown), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_LevelCountdown()
    {
        _currentLevelState = ELevelState.Countdown;
        OnLevelCountdown?.Invoke();

        StartCoroutine(CountdownTimerCO());
        Debug.LogWarning("Level countdown");
    }

    private IEnumerator CountdownTimerCO()
    {
        _countdownTimerText.gameObject.SetActive(true);

        while (_currentLevelState == ELevelState.Countdown)
        {
            _countdownTimerText.text = _countdownTimer.CurrentRemainingTime.ToString("n0");
            yield return null;
        }

        _countdownTimerText.gameObject.SetActive(false);
    }

    public void ServerLevelStart()
    {
        _levelTimer.StartTimerAsServer(_levelData.RoundDuration);
        _photonView.RPC(nameof(RPC_LevelStart), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_LevelStart()
    {
        _currentLevelState = ELevelState.Running;
        OnLevelCountdown?.Invoke();

        StartCoroutine(LevelTimerCO());
        Debug.LogWarning("Level start");
    }

    private IEnumerator LevelTimerCO()
    {
        _levelTimerText.gameObject.SetActive(true);

        while (_currentLevelState == ELevelState.Running)
        {
            _levelTimerText.text = _levelTimer.CurrentRemainingTime.ToString("n0");
            yield return null;
        }

        _levelTimerText.gameObject.SetActive(false);
    }

    public void ServerLevelEnd()
    {
        StartCoroutine(ServerLevelEndCO());
        _photonView.RPC(nameof(RPC_LevelEnd), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_LevelEnd()
    {
        _currentLevelState = ELevelState.Ended;
        OnLevelEnd?.Invoke();

        _roundEndText.gameObject.SetActive(true);
        Debug.LogWarning("Level end");
    }

    //Runs on master client only
    private IEnumerator ServerLevelEndCO()
    {
        yield return WaitHandler.GetWaitForSeconds(GameData.LEVEL_END_WAITING_TIME);
        _gameManager.OnLevelEnd();
    }

    //Game loop unrelated functions
    public Vector3 GetSpawnPoint()
    {
        return _team1SpawnPoint.position + new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0f, Random.Range(-_spawnRadius, _spawnRadius));
    }

}
