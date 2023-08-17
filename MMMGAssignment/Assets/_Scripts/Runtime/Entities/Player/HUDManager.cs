using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCells.Player
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private Image _healthImage;
        [SerializeField] private TMP_Text _eliminatedText;

        private void Awake()
        {
            if (!_playerManager.photonView.IsMine)
                Destroy(this.gameObject);

            UpdateUsername(_playerManager.photonView.Owner.NickName);
            _eliminatedText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _playerManager.OnHealthChanged += UpdateHealthUI;
            _playerManager.OnPlayerEliminated += ShowEliminatedUI;
        }

        private void OnDisable()
        {
            _playerManager.OnHealthChanged -= UpdateHealthUI;
            _playerManager.OnPlayerEliminated -= ShowEliminatedUI;
        }

        public void UpdateUsername(string username)
        {
            _usernameText.text = username;
        }

        public void UpdateHealthUI(float healthPercentage)
        {
            _healthImage.fillAmount = healthPercentage;
        }

        public void ShowEliminatedUI()
        {
            _eliminatedText.gameObject.SetActive(true);
        }
    }
}