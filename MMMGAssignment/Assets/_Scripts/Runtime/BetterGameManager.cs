using GameCells;
using GameCells.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterGameManager : Singleton<BetterGameManager>
{
    //TODO only exist on master client
    //TODO in future, handle host change via PunCallbacks
    
    private LevelRepository _levelRepository;
    private LevelRepository levelRepository => _levelRepository ??= LevelRepository.GetInstance();

    private int currentRoundNumber;

    private SO_Level _currentLevel = null;

    public event Action OnLevelFinishLoading;

    private Player[] _playerList;

    private void Awake()
    {
        //Initialization
        this.SetDontDestroyOnLoad();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        _playerList = PhotonNetwork.PlayerList;
        currentRoundNumber = 0;
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        currentRoundNumber++;
        _currentLevel = levelRepository.GetLevel(currentRoundNumber);

        StartCoroutine(LoadNextLevelCO());
    }

    private IEnumerator LoadNextLevelCO()
    {
        Debug.LogWarning($"Loading round {currentRoundNumber}: {_currentLevel.LevelName}");
        PhotonNetwork.LoadLevel(_currentLevel.SceneName);

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            //TODO Loading Screen Implementation

            Debug.Log($"Loading Scene: {PhotonNetwork.LevelLoadingProgress}");
            yield return null;
        }

        OnLevelFinishLoading?.Invoke();
    }

    public void OnLevelEnd()
    {
        if (currentRoundNumber == levelRepository.NumberOfLevels)
        {
            EndGame();
        }
        else
        {
            LoadNextLevel();
        }
    }

    public void EndGame()
    {
        Debug.LogError("Game ended");
    }
}
