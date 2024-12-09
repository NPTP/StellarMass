using UnityEngine;

namespace Summoner.Utilities.Singletons
{
    public class ClosedSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Micro-optimization instead of checking if the instance is null
        public static bool Exists { get; private set; }

        private static T instance;
        protected static T Instance
        {
            get
            {
                if (!Exists)
                    PopulateInstance();
                return instance;
            }
            private set
            {
                instance = value;
                Exists = instance != null;
            }
        }
        
        private void Awake()
        {
            if (instance == null)
            {
                PopulateInstance();
            }
            
            if (instance != this)
            {
                Destroy(this);
                return;
            }

            AwakeOverrideable();
        }

        private void OnDestroy()
        {
            if (instance != this)
            {
                return;
            }
            
            OnDestroyOverrideable();
            Instance = null;
        }

        protected virtual void AwakeOverrideable() { }

        protected virtual void OnDestroyOverrideable() { }

        private static void PopulateInstance()
        {
            Instance = FindObjectOfType<T>();
            if (!Exists)
                Instance = new GameObject(typeof(T).Name).AddComponent<T>();
        }
    }
}