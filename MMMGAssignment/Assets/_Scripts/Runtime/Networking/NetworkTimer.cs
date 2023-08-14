using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GameCells.PhotonNetworking
{
    public class NetworkTimer : MonoBehaviourPunCallbacks
    {
        public const string START_TIME = "StartTime";

        public event Action OnTimerEndEvent;

        public bool IsRunning { get; private set; }

        public int StartTime { get; private set; }
        public float Duration { get; private set; }
        public float TimeRemaining { get; private set; }

        private void Update()
        {
            if (!IsRunning)
                return;

            TimeRemaining = CalculateTimeRemaining();
            Debug.Log(TimeRemaining);

            if (TimeRemaining <= 0)
            {
                OnTimerEnd();
            }
        }

        private void OnTimerEnd()
        {
            IsRunning = false;
            Debug.Log("TIMER END");

            OnTimerEndEvent?.Invoke();
        }

        public void ServerStartTimer(float duration)
        {
            if (IsRunning)
            {
                Debug.LogError("Timer is already running!");
                return;
            }

            StartTime = PhotonNetwork.ServerTimestamp;

            Hashtable properties = new Hashtable()
            {
                {START_TIME, StartTime}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

            Duration = duration;
            IsRunning = true;
            Debug.Log($"Timer started with duration of {duration}");
        }

        public void ClientStartTimer(float duration)
        {
            if (IsRunning)
            {
                Debug.LogError("Timer is already running!");
                return;
            }

            Duration = duration;
            IsRunning = true;
            Debug.Log($"Timer started with duration of {duration}");
        }

        private float CalculateTimeRemaining()
        {
            int timer = PhotonNetwork.ServerTimestamp - this.StartTime;
            return this.Duration - timer / 1000f;
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            if (PhotonNetwork.IsMasterClient)
                return;

            if (!propertiesThatChanged.ContainsKey(START_TIME))
                return;

            ClientStartTimer((int)(propertiesThatChanged[START_TIME]));
        }
    }
}