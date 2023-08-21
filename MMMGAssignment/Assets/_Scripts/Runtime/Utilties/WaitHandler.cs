using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Utilities
{
    public class WaitHandler
    {
        private static readonly Dictionary<float, WaitForSeconds> waitForSecondsDict = new Dictionary<float, WaitForSeconds>();
        private static readonly Dictionary<float, WaitForSecondsRealtime> waitForSecondsRealtimeDict = new Dictionary<float, WaitForSecondsRealtime>();

        public static WaitForSeconds GetWaitForSeconds(float duration)
        {
            if (waitForSecondsDict.TryGetValue(duration, out WaitForSeconds wait)) return wait;

            waitForSecondsDict[duration] = new WaitForSeconds(duration);
            return waitForSecondsDict[duration];
        }

        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float duration)
        {
            if (waitForSecondsRealtimeDict.TryGetValue(duration, out WaitForSecondsRealtime wait)) return wait;

            waitForSecondsRealtimeDict[duration] = new WaitForSecondsRealtime(duration);
            return waitForSecondsRealtimeDict[duration];
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetWaitDictionaries()
        {
            waitForSecondsDict.Clear();
            Debug.Log("Dictionaries resetted.");
        }
    }
}