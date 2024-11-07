using UnityEngine;

namespace StellarMass.Systems.Coroutines
{
    public class CustomCoroutine
    {
        public enum LifecycleMode
        {
            Persistent = 0,
            SceneIsolated
        }
        
        private Coroutine coroutine;
        public LifecycleMode lifecycleMode = LifecycleMode.Persistent;

        public CustomCoroutine(Coroutine coroutine)
        {
            this.coroutine = coroutine;
        }
    }
}