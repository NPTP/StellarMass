using System;
using System.Collections;
using System.Collections.Generic;
using Summoner.Utilities.Singletons;
using UnityEngine;

namespace Summoner.Systems.Coroutines
{
    public class CoroutineOwner : ManualInitSingleton<CoroutineOwner>
    {
        private HashSet<CustomCoroutine> coroutines = new();

        public static CustomCoroutine StartRoutine(IEnumerator routine)
        {
            return new CustomCoroutine(Instance.StartCoroutine(routine));
        }

        private void OnCoroutineCompleted(CustomCoroutine coroutine)
        {
            coroutines.Remove(coroutine);
        }

        private IEnumerator CoroutineCallback(IEnumerator routine)
        {
            CustomCoroutine coroutine = new CustomCoroutine(StartCoroutine(routine));
            coroutines.Add(coroutine);
            yield return coroutine;
            OnCoroutineCompleted(coroutine);
        }
        
        public static void StopRoutine(IEnumerator routine)
        {
            Instance.StopCoroutine(routine);
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