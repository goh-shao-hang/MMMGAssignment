using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Utilities
{
    public static class StringValidator
    {
        public static bool ValidateString(string stringToValidate)
        {
            if (string.IsNullOrEmpty(stringToValidate))
            {
                return false;
            }

            return true;
        }
    }
}