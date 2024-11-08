using UnityEngine;

namespace StellarMass.Systems.Data.Persistent
{
    /// <summary>
    /// Scriptable object accessible at any point during runtime.
    /// Loads when requested, but never unloads, therefore - persistent!
    /// </summary>
    public sealed partial class PersistentData : DataScriptable
    {
        private static string Address => nameof(PersistentData);

        private static bool isLoaded;

        private static PersistentData instance;
        private static PersistentData Instance
        {
            get
            {
                if (!isLoaded)
                {
                    instance = Resources.Load<PersistentData>(Address);
                    isLoaded = true;
                }
                
                return instance;
            }
        }
        
        [SerializeField] private CorePersistentData core;
        public static CorePersistentData Core => Instance.core;

        private void OnEnable()
        {
            if (instance == null || instance == this)
            {
                return;
            }
            
            Destroy(this);
        }

        private void OnDisable()
        {
            if (instance != this)
            {
                return;
            }
            
            instance = null;
        }
    }
}