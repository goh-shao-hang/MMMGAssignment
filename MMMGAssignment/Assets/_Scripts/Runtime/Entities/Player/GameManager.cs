using GameCells.Player;
using GameCells.Utilities;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerManager _playerManagerPrefab;
    public PlayerManager playerManagerPrefab => _playerManagerPrefab;


    public event Action OnAllPlayersJoinedScene;

    //TODO rounds
    public int TotalRounds { get; private set; } = 3;
    public int CurrentRoundNumber { get; private set; }
    public bool IsLevelUnderProgress { get; private set; }

    private void Awake()
    {
        this.SetDontDestroyOnLoad();

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void StartGame()
    {
        CurrentRoundNumber = 0;
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        CurrentRoundNumber++;

        Debug.Log($"Starting Round {CurrentRoundNumber}");
        StartCoroutine(LoadLevelCO(PersistentRepository.GetInstance().GetRandomLevel().SceneName));
    }

    private IEnumerator LoadLevelCO(string levelName)
    {
        yield return new WaitForSeconds(3);

        //TODO
        if (!PhotonNetwork.IsMasterClient)
            yield break;

        PhotonNetwork.LoadLevel("test");

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            Debug.Log($"Loading Transition");
            yield return null;
        }

        PhotonNetwork.LoadLevel(levelName);

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            //TODO Loading Screen Implementation

            Debug.Log($"Loading Scene: {PhotonNetwork.LevelLoadingProgress}");
            yield return null;
        }

        ReadyLevel();
    }

    public void ReadyLevel()
    {
        OnAllPlayersJoinedScene?.Invoke();
    }

    public void EndCurrentLevel()
    {
        if (CurrentRoundNumber == TotalRounds)
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
        Destroy(this.gameObject);

        //TODO: go back to lobby
        Debug.Log("Thanks for playing");
    }
}
