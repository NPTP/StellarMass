using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class CoroutineOwner : ClosedSingleton<CoroutineOwner>
    {
        public static Coroutine StartRoutine(IEnumerator routine)
        {
            return PrivateInstance.StartCoroutine(routine);
        }
        
        public static void StopRoutine(IEnumerator routine)
        {
            PrivateInstance.StopCoroutine(routine);
        }

        public static void StopRoutine(Coroutine coroutine)
        {
            PrivateInstance.StopCoroutine(coroutine);
        }
    }
}