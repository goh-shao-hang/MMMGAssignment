using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLevel", menuName = "Data/Level Data")]
public class SO_Level : ScriptableObject
{
    [Header("Basic Info")]
    public string LevelName;
    public float RoundDuration;
    public string SceneName;
    public Sprite LevelOverviewImage;
    [TextArea]
    public string LevelDescription;

    [Header("Round Settings")]
    [Tooltip("If false, players are individuals")]
    public bool HasTeam;
    [Tooltip("If false, players become spectators on death")]
    public bool CanRespawn;
    
    public bool StartWithGun;

    [Header("Round End Settings")]
    public bool EndWhenPlayersEliminated;
    public int PlayersEliminationCount;
}
