using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Player
{
    public class PlayerAudio : MonoBehaviourPun
    {
        [SerializeField] private AudioSource _playerLoopAudioSource;
        [SerializeField] private AudioSource _playerSfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip _jumpSfx;
        [SerializeField] private AudioClip _movingSfx;
        [SerializeField] private AudioClip _fireSfx;
        [SerializeField] private AudioClip _hitSfx;

        [Header("Dependencies")]
        [SerializeField] private ThirdPersonMovement _playerMovement;
        [SerializeField] private PlayerShooting _playerShooting;
        [SerializeField] private PlayerHealth _playerHealth;

        [Header("Settings")]
        [SerializeField] private float _pitchVariation = 0.15f;

        private void OnEnable()
        {
            _playerMovement.OnStartMoving += PlayMovingSfx;
            _playerMovement.OnStopMoving += StopMovingSfx;
            _playerMovement.OnJump += PlayJumpSfx;
            _playerShooting.OnShoot += PlayShootSfx;
            _playerHealth.OnTakeDamage += PlayHitSfx;
        }

        private void PlayMovingSfx()
        {
            if (_playerLoopAudioSource.isPlaying)
                return;

            photonView.RPC(nameof(RPC_PlayMovingSfx), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_PlayMovingSfx()
        {
            _playerLoopAudioSource.clip = _movingSfx;
            _playerLoopAudioSource.Play();
        }

        private void StopMovingSfx()
        {
            photonView.RPC(nameof(RPC_StopMovingSfx), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_StopMovingSfx()
        {
            _playerLoopAudioSource.Stop();
        }

        private void PlayJumpSfx()
        {
            photonView.RPC(nameof(RPC_PlayJumpSfx), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_PlayJumpSfx()
        {
            _playerSfxSource.pitch = 1 + Random.Range(-_pitchVariation / 2, _pitchVariation / 2);
            _playerSfxSource.PlayOneShot(_jumpSfx);
        }

        private void PlayShootSfx()
        {
            photonView.RPC(nameof(RPC_PlayShootSfx), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_PlayShootSfx()
        {
            _playerSfxSource.pitch = 1 + Random.Range(-_pitchVariation / 2, _pitchVariation / 2);
            _playerSfxSource.PlayOneShot(_fireSfx);
        }

        private void PlayHitSfx()
        {
            photonView.RPC(nameof(RPC_PlayHitSfx), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_PlayHitSfx()
        {
            _playerSfxSource.pitch = 1 + Random.Range(-_pitchVariation / 2, _pitchVariation / 2);
            _playerSfxSource.PlayOneShot(_hitSfx);
        }
    }
}