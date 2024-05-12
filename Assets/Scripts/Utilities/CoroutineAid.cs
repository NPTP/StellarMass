using System;
using System.Collections;
using UnityEngine;

namespace StellarMass.Utilities
{
    public static class CoroutineAid
    {
        public static void Delay1Frame(Action callback) => DelayByFrames(1, callback);
        public static void DelayByFrames(int frames, Action callback)
        {
            frames = Mathf.Max(0, frames);
            CoroutineOwner.StartRoutine(delayRoutine());
            
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