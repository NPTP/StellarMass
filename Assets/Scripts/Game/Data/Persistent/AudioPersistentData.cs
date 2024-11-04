using FMODUnity;
using StellarMass.Systems.Data.Persistent;
using UnityEngine;

namespace StellarMass.Game.Data
{
    public sealed class AudioPersistentData : PersistentDataContainer
    {
        [Header("Parameters")]
        
        [SerializeField] [ParamRef]
        private string paramReference;
        public string ParamReference => paramReference;

        [Header("Music")]
        
        [SerializeField] private EventReference music;
        public EventReference Music => music;

        [Header("SFX")]
        
        [SerializeField] private EventReference ambience;
        public EventReference Ambience => ambience;
    }
}