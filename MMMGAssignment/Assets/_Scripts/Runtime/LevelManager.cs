using GameCells;
using GameCells.PhotonNetworking;
using GameCells.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LevelManager : SingletonPunCallbacks<LevelManager>
{
    [Header("Level Data (Required)")]
    [SerializeField] private SO_Level _levelData;
    public SO_Level LevelData => _levelData;

    [Header("Player Manager")]
    [SerializeField] private GameObject _playerManagerPrefab;

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
    [SerializeField] private CanvasGroup _levelIntroductionCanvas;
    [SerializeField] private Image _levelOverviewImage;
    [SerializeField] private TMP_Text _levelNameText;
    [SerializeField] private TMP_Text _levelDescriptionText;
    [SerializeField] private TMP_Text _countdownTimerText; 
    [SerializeField] private TMP_Text _levelTimerText;
    [SerializeField] private TMP_Text _roundEndText;
    [SerializeField] private GameObject _teamScoreUI;
    [SerializeField] private TMP_Text _blueTeamKillsText;
    [SerializeField] private TMP_Text _redTeamKillsText;

    private GameManager _gameManager;

    public ELevelState CurrentLevelState { get; private set; }

    private int _team1Kills = 0;
    private int _team2Kills = 0;

    //EVENTS
    public event Action OnLevelPreparing;
    public event Action OnLevelCountdown;
    public event Action OnLevelStart;
    public event Action OnLevelEnd;

    private void Awake()
    {
        //Called on everyone
        CurrentLevelState = ELevelState.Preparing;
        OnLevelPreparing?.Invoke();

        _countdownTimerText.gameObject.SetActive(false);
        _levelTimerText.gameObject.SetActive(false);

        _roundEndText.gameObject.SetActive(false);

        ShowLevelIntroduction();

        if (LevelData.HasTeam)
        {
            _teamScoreUI.SetActive(true);
        }
        else
        {
            _teamScoreUI.SetActive(false);
        }

        //Called on master client only
        if (!PhotonNetwork.IsMasterClient)
            return;

        _gameManager = GameManager.GetInstance();

        //Assign Teams
        if (LevelData.HasTeam)
        {
            AssignTeams();
        }
    }

    public override sealed void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        _gameManager.OnLevelFinishLoading += ServerLevelCountdown;

        //Subscribe to timers if is masterClient
        _countdownTimer.OnTimerExpired += ServerLevelStart;
        _levelTimer.OnTimerExpired += ServerLevelEnd;
    }

    public override sealed void OnDisable()
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
        StartCoroutine(DelayedServerLevelCountdownCO());
    }

    private IEnumerator DelayedServerLevelCountdownCO()
    {
        yield return WaitHandler.GetWaitForSeconds(GameData.LEVEL_INTRODUCTION_TIME);

        _countdownTimer.StartTimerAsServer(GameData.LEVEL_COUNTDOWN_TIME);
        _photonView.RPC(nameof(RPC_LevelCountdown), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_LevelCountdown()
    {
        CurrentLevelState = ELevelState.Countdown;

        FadeLevelIntroduction();

        StartCoroutine(CountdownTimerCO());
        Debug.LogWarning("Level countdown");

        //Spawn Player
        SpawnLocalPlayerManager();

        OnLevelCountdown?.Invoke();
    }

    private IEnumerator CountdownTimerCO()
    {
        _countdownTimerText.gameObject.SetActive(true);

        while (CurrentLevelState == ELevelState.Countdown)
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
        CurrentLevelState = ELevelState.Running;

        StartCoroutine(LevelTimerCO());
        Debug.LogWarning("Level start");

        OnLevelStart?.Invoke();
    }

    private IEnumerator LevelTimerCO()
    {
        _levelTimerText.gameObject.SetActive(true);

        while (CurrentLevelState == ELevelState.Running)
        {
            _levelTimerText.text = _levelTimer.CurrentRemainingTime.ToString("n0");
            yield return null;
        }

        _levelTimerText.gameObject.SetActive(false);
    }

    public void ServerLevelEnd()
    {
        _levelTimer.StopTimer();

        StartCoroutine(ServerLevelEndCO());
        _photonView.RPC(nameof(RPC_LevelEnd), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_LevelEnd()
    {
        CurrentLevelState = ELevelState.Ended;

        _roundEndText.gameObject.SetActive(true);
        Debug.LogWarning("Level end");

        OnLevelEnd?.Invoke();
    }

    //Runs on master client
    private IEnumerator ServerLevelEndCO()
    {
        _photonView.RPC(nameof(StopTime), RpcTarget.All);

        yield return WaitHandler.GetWaitForSecondsRealtime(GameData.LEVEL_END_WAITING_TIME);

        _photonView.RPC(nameof(ResumeTime), RpcTarget.All);

        _gameManager.OnLevelEnd();
    }

    [PunRPC]
    private void StopTime()
    {
        Time.timeScale = 0f;
    }

    [PunRPC]
    private void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    //Game loop unrelated functions
    private void SpawnLocalPlayerManager()
    {
        PhotonNetwork.Instantiate(_playerManagerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public Vector3 GetTeam1SpawnPoint()
    {
        return _team1SpawnPoint.position + new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0f, Random.Range(-_spawnRadius, _spawnRadius));
    }

    public Vector3 GetTeam2SpawnPoint()
    {
        return _team2SpawnPoint.position + new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0f, Random.Range(-_spawnRadius, _spawnRadius));
    }

    public void OnTeammateKilled(int killedTeamNumber)
    {
        photonView.RPC(nameof(RPC_OnTeammateKilled), RpcTarget.All, killedTeamNumber);
    }

    [PunRPC]
    private void RPC_OnTeammateKilled(int killedTeamNumber)
    {
        if (killedTeamNumber == 1)
        {
            _team2Kills++;
            _redTeamKillsText.text = _team2Kills.ToString();
        }
        else if (killedTeamNumber == 2)
        {
            _team1Kills++;
            _blueTeamKillsText.text = _team1Kills.ToString();
        }
    }

    private void ShowLevelIntroduction()
    {
        _levelIntroductionCanvas.gameObject.SetActive(true);
        _levelIntroductionCanvas.alpha = 1;

        _levelOverviewImage.overrideSprite = LevelData.LevelOverviewImage;
        _levelNameText.text = LevelData.LevelName;
        _levelDescriptionText.text = LevelData.LevelDescription;
    }

    private void FadeLevelIntroduction()
    {
        StartCoroutine(FadeLevelIntroductionCO());
    }

    private IEnumerator FadeLevelIntroductionCO()
    { 
        float timeElapsed = 0f;
        while (timeElapsed < GameData.LEVEL_INTRODUCTION_FADE_TIME)
        {
            timeElapsed += Time.deltaTime;
            _levelIntroductionCanvas.alpha = Mathf.Lerp(1, 0, timeElapsed / GameData.LEVEL_INTRODUCTION_FADE_TIME);
            yield return null;
        }

        _levelIntroductionCanvas.gameObject.SetActive(false);
    }



    private void AssignTeams()
    {
        List<Player> playerList = PhotonNetwork.PlayerList.ToList();

        int team1Count = 0;
        int team2Count = 0;

        while (playerList.Count > 0)
        {
            int index = Random.Range(0, playerList.Count);
            int teamNumber = 0;
            Player randomPlayer = playerList[index];

            if (team1Count < team2Count || team1Count == team2Count && Random.value < 0.5f)
            {
                team1Count++;
                teamNumber = 1;
            }
            else
            {
                team2Count++;
                teamNumber = 2;
            }

            Hashtable teamInfo = new Hashtable()
            {
                { GameData.TEAM_INFO_HASH, teamNumber }
            };

            randomPlayer.SetCustomProperties(teamInfo);
            playerList.RemoveAt(index);
        }
    }

}
