using System;
using System.Collections;
using UnityEngine;

namespace Summoner.Systems.Coroutines
{
    public static class CoroutinePlusExtensions
    {
        public static CoroutinePlus OnComplete(this CoroutinePlus coroutinePlus, Action callback)
        {
            if (coroutinePlus == null)
            {
                return null;
            }
            
            Coroutine nestedCoroutine = coroutinePlus.coroutine;
            Coroutine newCoroutine;
            
            switch (coroutinePlus.LifeCycle)
            {
                case LifeCycle.MonoBehaviour:
                    newCoroutine = coroutinePlus.MonoBehaviour.StartCoroutine(CallbackRoutine(nestedCoroutine, callback));
                    coroutinePlus.coroutine = newCoroutine;
                    break;
                case LifeCycle.Persistent:
                    newCoroutine = CoroutineOwner.StartRoutine(CallbackRoutine(nestedCoroutine, callback));
                    coroutinePlus.coroutine = newCoroutine;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return coroutinePlus;
        }

        private static IEnumerator CallbackRoutine(Coroutine nestedCoroutine, Action callback)
        {
            // TODO: Tighter frame method with iterator instead of yield return coroutine?
            // while (nestedIterator.MoveNext())
            // {
            //     yield return nestedIterator.Current;
            // }

            yield return nestedCoroutine;
            callback?.Invoke();
        }

        public static void Stop(this CoroutinePlus coroutinePlus)
        {
            if (coroutinePlus == null)
            {
                return;
            }
            
            switch (coroutinePlus.LifeCycle)
            {
                case LifeCycle.MonoBehaviour:
                    coroutinePlus.MonoBehaviour.StopCoroutine(coroutinePlus.coroutine);
                    break;
                case LifeCycle.Persistent:
                    CoroutineOwner.StopRoutine(coroutinePlus.coroutine);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static CoroutinePlus StopAndReplace(this CoroutinePlus coroutinePlus, IEnumerator routine)
        {
            if (coroutinePlus == null)
            {
                return null;
            }

            switch (coroutinePlus.LifeCycle)
            {
                case LifeCycle.MonoBehaviour:
                    if (coroutinePlus.coroutine != null)
                    {
                        coroutinePlus.MonoBehaviour.StopCoroutine(coroutinePlus.coroutine);
                    }
                    coroutinePlus.coroutine = coroutinePlus.MonoBehaviour.StartCoroutine(routine);
                    return coroutinePlus;
                case LifeCycle.Persistent:
                    if (coroutinePlus.coroutine != null)
                    {
                        CoroutineOwner.StopRoutine(coroutinePlus.coroutine);
                    }
                    coroutinePlus.coroutine = CoroutineOwner.StartRoutine(routine);
                    return coroutinePlus;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}