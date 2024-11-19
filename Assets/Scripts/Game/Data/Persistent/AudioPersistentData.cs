using FMODUnity;
using Summoner.Systems.Data.Persistent;
using UnityEngine;

namespace Summoner.Game.Data
{
    public sealed class AudioPersistentData : PersistentDataContainer
    {
        [Header("SFX")]
        
        [SerializeField] private EventReference ambience;
        public EventReference Ambience => ambience;
        
        [Header("SFX/Menu")]

        [SerializeField] private EventReference select;
        public EventReference Select => select;
    }
}