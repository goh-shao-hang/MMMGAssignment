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
        [SerializeField] private TMP_Text _respawningText;

        private void Awake()
        {
            if (!_playerManager.photonView.IsMine)
                Destroy(this.gameObject);

            UpdateUsername(_playerManager.photonView.Owner.NickName);
            _eliminatedText.gameObject.SetActive(false);
            _respawningText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _playerManager.OnHealthChanged += UpdateHealthUI;
            _playerManager.OnPlayerEliminated += ShowEliminatedUI;
            _playerManager.OnPlayerRespawnStart += ShowRespawningUI;
            _playerManager.OnPlayerRespawnTimeUpdate += UpdateRespawningUI;
            _playerManager.OnPlayerRespawnEnd += HideRespawningUI;
        }

        private void OnDisable()
        {
            _playerManager.OnHealthChanged -= UpdateHealthUI;
            _playerManager.OnPlayerEliminated -= ShowEliminatedUI;
            _playerManager.OnPlayerRespawnStart -= ShowRespawningUI;
            _playerManager.OnPlayerRespawnTimeUpdate -= UpdateRespawningUI;
            _playerManager.OnPlayerRespawnEnd -= HideRespawningUI;
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

        public void ShowRespawningUI()
        {
            _respawningText.gameObject.SetActive(true);
        }

        public void UpdateRespawningUI(float time)
        {
            _respawningText.text = $"Respawning in {(GameData.RESPAWN_TIME - time + 1).ToString("n0")}"; //+1 to indicate seconds remaining
        }

        public void HideRespawningUI()
        {
            _respawningText.gameObject.SetActive(false);
        }

    }
}