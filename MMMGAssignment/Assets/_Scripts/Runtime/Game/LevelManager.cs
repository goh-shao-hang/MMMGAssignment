using GameCells.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private SO_Level _levelData;

    public ELevelState _levelState { get; private set; }

    private void Start()
    {
        this._levelState = ELevelState.Preparing;
    }

}
