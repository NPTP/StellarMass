using UnityEngine;

namespace StellarMass.Systems.Data.Persistent
{
    public sealed class CorePersistentData : PersistentDataContainer
    {
        [Header("Save/Load")]
        
        [SerializeField] private bool saveOnApplicationExit;
        public bool SaveOnApplicationExit => saveOnApplicationExit;
    }
}