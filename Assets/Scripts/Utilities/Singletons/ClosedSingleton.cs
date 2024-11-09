using UnityEngine;

namespace Summoner.Utilities.Singletons
{
    public class ClosedSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static bool InstanceExists { get; private set; }

        private static T instance;
        protected static T Instance
        {
            get
            {
                if (!InstanceExists)
                    PopulateInstance();
                return instance;
            }
            private set
            {
                instance = value;
                InstanceExists = instance != null;
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
            if (!InstanceExists)
                Instance = new GameObject(typeof(T).Name).AddComponent<T>();
        }
    }
}