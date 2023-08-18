using GameCells;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorManager : MonoBehaviourPun
{
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    private void Start()
    {
        if (photonView.Owner.CustomProperties.ContainsKey(GameData.PLAYER_COLOR_HASH))
        {
            int textureIndex = (int)(photonView.Owner.CustomProperties[GameData.PLAYER_COLOR_HASH]);
            meshRenderer.material.mainTexture
                = PlayerColorRepository.GetInstance().GetTextureByIndex(textureIndex);
        }
    }
}
