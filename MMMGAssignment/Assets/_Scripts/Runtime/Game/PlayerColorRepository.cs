using GameCells.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorRepository : Singleton<PlayerColorRepository>
{
    [Header("Colors")]
    [SerializeField] private Texture[] _playerTextures;
    public Texture[] PlayerTextures => _playerTextures;

    [Header("Color Overrides When In Team")]
    [SerializeField] private Texture _blueTexture;
    [SerializeField] private Texture _redTexture;

    public Texture BlueTexture => _blueTexture;
    public Texture RedTexture => _redTexture;


    private void Awake()
    {
        this.SetDontDestroyOnLoad();
    }

    public Texture GetTextureByIndex(int index)
    {
        return _playerTextures[index];
    }
}
