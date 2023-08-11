using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Utilities
{
    public static class Helper
    {
        public static bool CompareLayer(GameObject gameObject, LayerMask layerMask)
        {
            if (((1 << gameObject.layer) & layerMask) != 0)
            {
                //Is in one of the layers
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}