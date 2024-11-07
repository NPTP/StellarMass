using UnityEngine;

namespace StellarMass.Utilities.Singletons
{
    public abstract class ManualInitSingleton : MonoBehaviour { }

    /// <summary>
    /// "ManualInit" because Initialize must be called on it before it can be used.
    /// Awake and start logic are not to be implemented so that the order of operations
    /// is manually controlled. In addition, the static instance is not publicly accessible,
    /// which requires implementation to use only statically accessible methods & properties
    /// (and access the instance privately). This is because this class is intended for Singletons
    /// which are initialized intentionally on startup, and persist through the entire play session,
    /// so they look like and can be treated like static classes.
    /// </summary>
    public class ManualInitSingleton<T> : ManualInitSingleton where T : MonoBehaviour
    {
        protected static T Instance { get; private set; }

        private void Awake()
        {
            // Not to be overriden.
        }

        private void OnDestroy()
        {
            // Not to be overriden.
        }
        
        public static void Initialize()
        {
            if (Instance != null)
            {
                return;
            }
            
            Instance = FindObjectOfType<T>();
            if (Instance == null)
                Instance = new GameObject(typeof(T).Name).AddComponent<T>();
        }
    }
}