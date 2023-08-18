using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells
{
    public class GameData : MonoBehaviour
    {
        //TODO ROUNDS
        public const int ROUNDS_PER_GAME = 3;

        public const float LEVEL_COUNTDOWN_TIME = 3f;
        public const float LEVEL_END_WAITING_TIME = 3f;
        public const float RESPAWN_TIME = 3f;
        public const float LEVEL_INTRODUCTION_TIME = 8f;
        public const float LEVEL_INTRODUCTION_FADE_TIME = 1f;

        #region Hashes

        public const string PLAYER_COLOR_HASH = "PlayerColor";

        #endregion

        #region Animator Hashes

        public static readonly int IS_MOVING_HASH = Animator.StringToHash("isMoving");
        public static readonly int JUMP_HASH = Animator.StringToHash("jump");

        #endregion
    }
}