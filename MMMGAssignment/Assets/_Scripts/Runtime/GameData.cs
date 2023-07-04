using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells
{
    public class GameData : MonoBehaviour
    {
        #region Animator Hashes

        public static readonly int IS_MOVING_HASH = Animator.StringToHash("isMoving");
        public static readonly int JUMP_HASH = Animator.StringToHash("jump");

        #endregion
    }
}