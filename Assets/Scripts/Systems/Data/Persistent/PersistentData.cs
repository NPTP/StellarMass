using Summoner.Game.Data;
using Summoner.Game.Data.Persistent;
using UnityEngine;

namespace Summoner.Systems.Data.Persistent
{
    /// <summary>
    /// Scriptable object accessible at any point during runtime.
    /// Loads when requested, but never unloads, therefore - persistent!
    /// </summary>
    public sealed class PersistentData : DataScriptable
    {
        private static string Address => nameof(PersistentData);

        private static bool isLoaded;
        private static bool isDisabled;

        private static PersistentData instance;
        private static PersistentData Instance
        {
            get
            {
                if (!isLoaded && !isDisabled)
                {
                    instance = Resources.Load<PersistentData>(Address);
                    isLoaded = true;
                }
                
                return instance;
            }
        }
        
        [SerializeField] private CorePersistentData core;
        public static CorePersistentData Core => Instance.core;
        
        [SerializeField] private GamePersistentData game;
        public static GamePersistentData Game => Instance.game;
        
        [SerializeField] private PlayerPersistentData player;
        public static PlayerPersistentData Player => Instance.player;
        
        [SerializeField] private AudioPersistentData audio;
        public static AudioPersistentData Audio => Instance.audio;
        
        [SerializeField] private CameraPersistentData camera;
        public static CameraPersistentData Camera => Instance.camera;

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
            isLoaded = false;
            isDisabled = true;
        }
    }
}