using GameCells;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorManager : MonoBehaviourPun
{
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    private LevelManager _levelManager;
    private LevelManager levelManager => _levelManager ??= LevelManager.GetInstance();

    private PlayerColorRepository _playerColorRepository;
    private PlayerColorRepository playerColorRepository => _playerColorRepository ??= PlayerColorRepository.GetInstance();


    private bool _overrideColor = false;

    private void Start()
    {
        if (_overrideColor)
            return;

        if (photonView.Owner.CustomProperties.ContainsKey(GameData.PLAYER_COLOR_HASH))
        {
            int textureIndex = (int)(photonView.Owner.CustomProperties[GameData.PLAYER_COLOR_HASH]);
            meshRenderer.material.mainTexture
                = playerColorRepository.GetTextureByIndex(textureIndex);
        }
    }

    public void OverrideColor(int teamNumber)
    {
        photonView.RPC(nameof(RPC_OverrideColor), RpcTarget.All, teamNumber);
    }

    [PunRPC]
    private void RPC_OverrideColor(int teamNumber)
    {
        _overrideColor = true;

        if (teamNumber == 1)
        {
            meshRenderer.material.mainTexture = playerColorRepository.BlueTexture;
        }
        else
        {
            meshRenderer.material.mainTexture = playerColorRepository.RedTexture;
        }
    }
}
