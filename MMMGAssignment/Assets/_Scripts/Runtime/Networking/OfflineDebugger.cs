using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineDebugger : MonoBehaviour
{
    [SerializeField] private bool _startInOfflineMode = false;

    private void Awake()
    {
        PhotonNetwork.OfflineMode = _startInOfflineMode;
    }

    private void Update()
    {
        Debug.Log(PhotonNetwork.OfflineMode);
    }
}
