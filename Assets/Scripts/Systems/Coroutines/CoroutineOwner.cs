using System;
using System.Collections;
using Summoner.Utilities.Singletons;
using UnityEngine;

namespace Summoner.Systems.Coroutines
{
    public class CoroutineOwner : ManualInitSingleton<CoroutineOwner>
    {
        public static Coroutine StartRoutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }

        public static void StopRoutine(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }
        
        public static void Delay1Frame(Action callback) => DelayByFrames(1, callback);
        public static void DelayByFrames(int frames, Action callback)
        {
            frames = Mathf.Max(0, frames);
            StartRoutine(delayRoutine());
            
            IEnumerator delayRoutine()
            {
                for (int f = 0; f < frames; f++)
                {
                    yield return null;
                }
                
                callback?.Invoke();
            }
        }
    }
}