using UnityEngine;

namespace StellarMass.Systems.Data.Persistent
{
    public sealed class CorePersistentData : PersistentDataContainer
    {
        [Header("FMOD Buses")]
        
        [SerializeField] private string masterBusPath;
        public string MasterBusPath => masterBusPath;
        
        [SerializeField] private string musicBusPath;
        public string MusicBusPath => musicBusPath;
        
        [SerializeField] private string sfxBusPath;
        public string SfxBusPath => sfxBusPath;
        
        [Header("Save/Load")]
        
        [SerializeField] private bool saveOnApplicationExit;
        public bool SaveOnApplicationExit => saveOnApplicationExit;
    }
}