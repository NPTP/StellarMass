using UnityEngine;

namespace Utilities
{
    public class ClosedSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        protected static T PrivateInstance
        {
            get
            {
                if (instance == null)
                {
                    PopulateInstance();
                }
                return instance;
            }
        }

        public static bool IsNull => instance == null;

        private void Awake()
        {
            if (instance != null & instance != this)
            {
                Destroy(this);
                return;
            }

            PopulateInstance();
            AwakeInitialize();
        }

        private void OnDestroy()
        {
            if (instance != this)
            {
                return;
            }
            
            OnDestroyTerminate();
            instance = null;
        }

        protected virtual void AwakeInitialize()
        {
        }

        protected virtual void OnDestroyTerminate()
        {
        }

        private static void PopulateInstance()
        {
            instance = FindObjectOfType<T>();
        
            if (instance == null)
            {
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }
        }
    }
}