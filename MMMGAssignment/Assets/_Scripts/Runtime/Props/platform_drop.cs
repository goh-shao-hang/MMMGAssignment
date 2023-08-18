using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCells.Utilities;
using Photon.Pun;

public class platform_drop : MonoBehaviourPun
{
    [SerializeField] private float waitingTime = 6f;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(waiter());
        }
    }

    private IEnumerator waiter()
    {
        yield return WaitHandler.GetWaitForSeconds(waitingTime);
        photonView.RPC(nameof(RPC_DestroySelf), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_DestroySelf()
    {
        Object.Destroy(this.gameObject);
    }
}
