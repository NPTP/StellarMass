using FMODUnity;
using StellarMass.Systems.Data;
using UnityEngine;

namespace StellarMass.Game.Data
{
    [CreateAssetMenu]
    public class AudioData : RuntimeData<AudioData>
    {
        [Header("Parameters")]
        
        [SerializeField] [ParamRef]
        private string paramReference;
        public static string ParamReference => Instance.paramReference;

        [Header("Music")]
        
        [SerializeField] private EventReference music;
        public static EventReference Music => Instance.music;

        [Header("SFX")]
        
        [SerializeField] private EventReference ambience;
        public static EventReference Ambience => Instance.ambience;
    }
}