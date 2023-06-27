using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Utilities
{
    public static class RandomUtil
    {
        /// <summary>
        /// Return true or false based on a successChance between 0 and 1.
        /// </summary>
        /// <param name="successChance"></param>
        /// <returns></returns>
        public static bool SuccessRateCheck(float successChance)
        {
            float random = Random.value;
            if (successChance >= random)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}