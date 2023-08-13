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


    public event Action OnLevelReady;
    public event Action OnLevelEnd;

    public int CurrentRoundNumber { get; private set; }
    public bool IsLevelUnderProgress { get; private set; }

    private void Awake()
    {
        this.SetDontDestroyOnLoad();
    }

    public void StartGame()
    {
        StartCoroutine(LoadLevelCO(PersistentRepository.GetInstance().GetRandomLevel().SceneName));
    }

    private IEnumerator LoadLevelCO(string levelName)
    {
        PhotonNetwork.LoadLevel(levelName);

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            //Loading Screen Implementation

            Debug.Log(PhotonNetwork.LevelLoadingProgress);
            yield return null;
        }

        StartCurrentLevel();
    }

    public void StartCurrentLevel()
    {
        OnLevelReady?.Invoke();
    }

    public void EndCurrentLevel()
    {

    }

    public void EndGame()
    {
        Destroy(this.gameObject);
    }
}
