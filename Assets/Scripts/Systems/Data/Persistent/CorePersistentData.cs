using UnityEngine;

namespace Summoner.Systems.Data.Persistent
{
    public sealed class CorePersistentData : PersistentDataContainer
    {
        [Header("Build Versioning")]
        [SerializeField] private int majorVersion = 0;
        public int MajorVersion => majorVersion;

        [SerializeField] private int minorVersion = 0;
        public int MinorVersion => minorVersion;

        [SerializeField] private int patchVersion = 0;
        public int PatchVersion => patchVersion;
        
        [Header("Save/Load")]
        
        [SerializeField] private bool saveOnApplicationExit;
        public bool SaveOnApplicationExit => saveOnApplicationExit;
    }
}