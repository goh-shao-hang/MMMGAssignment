using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCells.Player
{
    public class PlayerHUDManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerHealth _playerHealth;

        [SerializeField] private Image _healthImage;

        private void OnEnable()
        {
            _playerHealth.OnHealthChanged += UpdateHealthUI;
        }

        private void OnDisable()
        {
            _playerHealth.OnHealthChanged -= UpdateHealthUI;
        }

        public void UpdateHealthUI(float healthPercentage)
        {
            _healthImage.fillAmount = healthPercentage;
        }
        
    }
}