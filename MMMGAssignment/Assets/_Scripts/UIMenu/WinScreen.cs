using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.EnableCloseConnection = true;
        PhotonNetwork.AutomaticallySyncScene = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
