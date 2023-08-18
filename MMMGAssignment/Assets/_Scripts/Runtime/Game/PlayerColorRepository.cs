using GameCells.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorRepository : Singleton<PlayerColorRepository>
{
    [SerializeField] private Texture[] _playerTextures;
    public Texture[] PlayerTextures => _playerTextures;

    private void Awake()
    {
        this.SetDontDestroyOnLoad();
    }

    public Texture GetTextureByIndex(int index)
    {
        return _playerTextures[index];
    }
}
