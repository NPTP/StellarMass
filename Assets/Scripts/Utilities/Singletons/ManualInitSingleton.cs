using UnityEngine;

namespace Summoner.Utilities.Singletons
{
    /// <summary>
    /// "ManualInit" because Initialize must be called on it before it can be used.
    /// Awake and start logic are not to be implemented so that the order of operations
    /// is manually controlled. For optimization, there are also no protections on duplicate
    /// instances, so this must be used carefully and specifically. All such singletons should
    /// exist only in the bootstrap scene.
    ///
    /// In addition, the static instance is not publicly accessible,
    /// which requires implementation to use only statically accessible methods & properties
    /// (and access the instance privately). This is because this class is intended for Singletons
    /// which are initialized intentionally on startup, and persist through the entire play session,
    /// so they look like and can be treated like static classes.
    /// </summary>
    public abstract class ManualInitSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T Instance { get; private set; }

        // There are not to be overridden.
        private void Awake() { }
        private void OnEnable() { }

        public void Initialize()
        {
            if (Instance != null)
            {
                return;
            }
            
            Instance = this as T;
            InitializeOverrideable();
        }

        protected virtual void InitializeOverrideable() { }
    }
}