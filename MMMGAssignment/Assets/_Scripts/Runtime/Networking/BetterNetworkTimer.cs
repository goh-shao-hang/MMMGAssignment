using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BetterNetworkTimer : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _timerName;

    public event Action OnTimerExpired;

    public bool IsRunning { get; private set; }

    private float _timerDuration;
    private int _startTimestamp;
    public float CurrentRemainingTime { get; private set; }

    private void Update()
    {
        if (!IsRunning)
            return;

        float timePassed = (PhotonNetwork.ServerTimestamp - _startTimestamp) / 1000; //Convert milliseconds to seconds

        CurrentRemainingTime = _timerDuration - timePassed;

        if (CurrentRemainingTime <= 0)
        {
            OnTimerEnd();
        }
    }

    public void StartTimerAsServer(float timerDuration)
    {
        if (IsRunning)
        {
            Debug.LogError($"You are trying to start {_timerName} but it is already running!");
            return;
        }

        _startTimestamp = PhotonNetwork.ServerTimestamp;

        //Set properties
        object[] timerInfo = new object[2];
        timerInfo[0] = _startTimestamp; //first parameter is start time
        timerInfo[1] = timerDuration; //second parameter is duration

        Hashtable timerProps = new Hashtable()
        {
            { _timerName, timerInfo }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(timerProps);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (!propertiesThatChanged.ContainsKey(_timerName))
            return;

        //The timer is set here, both server and clients
        object[] receivedTimerInfo = propertiesThatChanged[_timerName] as object[];
        this._startTimestamp = (int)(receivedTimerInfo[0]); //Server already know this but tell everyone anyways
        this._timerDuration = (float)(receivedTimerInfo[1]);

        ActivateTimer();
    }

    private void ActivateTimer()
    {
        float timePassed = (PhotonNetwork.ServerTimestamp - _startTimestamp) / 1000; //Convert milliseconds to seconds

        CurrentRemainingTime = _timerDuration - timePassed;

        if (CurrentRemainingTime > 0)
        {
            IsRunning = true;
        }
        else
        {
            OnTimerEnd();
        }
    }

    private void OnTimerEnd()
    {
        IsRunning = false;
        OnTimerExpired?.Invoke();
    }
}
