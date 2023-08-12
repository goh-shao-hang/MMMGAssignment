using GameCells.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRepository : Singleton<LevelRepository>
{
    [SerializeField] private SO_Level[] _levels;

    private void Awake()
    {
        this.SetDontDestroyOnLoad();
    }

    public SO_Level GetRandomLevel()
    {
        return _levels[Random.Range(0, _levels.Length)];
    }
}
