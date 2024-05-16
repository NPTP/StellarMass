using StellarMass.Utilities;
using UnityEngine;

namespace StellarMass.Systems.Data
{
    public abstract class RuntimeData : DataScriptable { }
    
    public abstract class RuntimeData<T> : RuntimeData where T : ScriptableObject
    {
        private static string Address => typeof(T).Name;
        
        private static T instance;
        protected static T Instance
        {
            get
            {
                if (instance == null)
                    instance = AddressablesUtility.LoadAssetSynchronous<T>(Address);
                return instance;
            }
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            this.EDITOR_SetAddress(Address, createIfNotAddressable: true);
#endif
        }

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