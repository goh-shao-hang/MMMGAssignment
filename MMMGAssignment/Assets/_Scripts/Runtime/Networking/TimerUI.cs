using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameCells.PhotonNetworking
{
    public class TimerUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private NetworkTimer _networkTimer;
        [SerializeField] private TMP_Text _text;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Update()
        {
        }
    }
}