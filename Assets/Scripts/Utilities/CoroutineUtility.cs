using System.Collections;
using UnityEngine;

namespace Summoner.Utilities
{
    public static class CoroutineUtility
    {
        public static Coroutine StopAndReplaceCoroutine(MonoBehaviour monoBehaviour, ref Coroutine coroutine, IEnumerator routine)
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
            }

            coroutine = monoBehaviour.StartCoroutine(routine);
            return coroutine;
        }

        public static void StopAndNullCoroutine(MonoBehaviour monoBehaviour, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}