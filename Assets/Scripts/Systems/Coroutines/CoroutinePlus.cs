using System.Collections;
using UnityEngine;

namespace Summoner.Systems.Coroutines
{
    public class CoroutinePlus
    {
        public Coroutine coroutine;

        public MonoBehaviour MonoBehaviour { get; }
        public LifeCycle LifeCycle { get; }

        private CoroutinePlus(Coroutine coroutine, MonoBehaviour monoBehaviour, LifeCycle lifeCycle)
        {
            this.coroutine = coroutine;
            MonoBehaviour = monoBehaviour;
            LifeCycle = lifeCycle;
        }

        public static CoroutinePlus StartPersistent(IEnumerator routine)
        {
            return new CoroutinePlus(CoroutineOwner.StartRoutine(routine), null, LifeCycle.Persistent);
        }

        public static CoroutinePlus Start(IEnumerator routine, MonoBehaviour monoBehaviour)
        {
            return new CoroutinePlus(monoBehaviour.StartCoroutine(routine), monoBehaviour, LifeCycle.MonoBehaviour);
        }
    }
}