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
        [SerializeField] private string _timerName;

        public event Action OnTimerStart;
        public event Action OnTimerExpired;

        public const string TIMER_DURATION_HASH = "TimerDuration";

        public bool IsRunning { get; private set; }

        private float _duration;
        private int _timerStartTimestamp;
        private float _currentRemainingTime;

        public float CurrentRemainingTime => _currentRemainingTime;

        public override void OnEnable()
        {
            base.OnEnable();

            if (IsRunning)
                return;

            //Prevent late connectors from missing the timer
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            if (roomProperties.ContainsKey(_timerName) && roomProperties.ContainsKey(TIMER_DURATION_HASH))
            {
                this._duration = (float)(roomProperties[TIMER_DURATION_HASH]);

                InitializeTimer((int)roomProperties[_timerName]);
            }
        }

        private void Update()
        {
            if (!IsRunning)
                return;

            _currentRemainingTime = CalculateRemainingTime();

            if (_currentRemainingTime <= 0)
                EndTimer();
        }

        private void StartTimer()
        {
            IsRunning = true;

            OnTimerStart?.Invoke();
        }

        private void EndTimer()
        {
            IsRunning = false;

            OnTimerExpired?.Invoke();
        }

        //Called by Master Client only
        public void ServerStartTimer(float duration)
        {
            if (IsRunning)
            {
                Debug.LogError("Timer is already running!");
                return;
            }

            this._timerStartTimestamp = PhotonNetwork.ServerTimestamp;

            Hashtable properties = new Hashtable()
            {
                {_timerName, _timerStartTimestamp},
                {TIMER_DURATION_HASH, duration}
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            if (propertiesThatChanged.ContainsKey(_timerName) && propertiesThatChanged.ContainsKey(TIMER_DURATION_HASH))
            {
                this._duration = (float)(propertiesThatChanged[TIMER_DURATION_HASH]);

                InitializeTimer((int)propertiesThatChanged[_timerName]);
            }
        }

        private void InitializeTimer(int startTimestamp)
        {
            this._timerStartTimestamp = startTimestamp;

            if (CalculateRemainingTime() > 0)
            {
                Debug.LogError($"{_timerName} started, {IsRunning}");
                StartTimer();
            }
            else
            {
                EndTimer();
            }
        }

        private float CalculateRemainingTime()
        {
            float timePassed = (PhotonNetwork.ServerTimestamp - _timerStartTimestamp) / 1000; //Convert milliseconds to seconds

            return _duration - timePassed;
        }
    }
}