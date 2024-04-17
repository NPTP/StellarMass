using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class CoroutineOwner : Singleton<CoroutineOwner>
    {
        public static Coroutine StartRoutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }
        
        public static void StopRoutine(IEnumerator routine)
        {
            Instance.StopCoroutine(routine);
        }

        public static void StopRoutine(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }
    }
}