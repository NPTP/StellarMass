using StellarMass.Systems.Data.Persistent;
using UnityEngine;

namespace StellarMass.Game.Data
{
    [CreateAssetMenu]
    public sealed class CameraPersistentData : PersistentDataContainer
    {
        [Header("Local Bobbing Tuning")]
        [SerializeField] private float vertSinAmp = 1;
        public float VertSinAmp => vertSinAmp;
        
        [SerializeField] private float vertSinFreq = 1;
        public float VertSinFreq => vertSinFreq;
        
        [SerializeField] private float horSinAmp = 1;
        public float HorSinAmp => horSinAmp;
        
        [SerializeField] private float horSinFreq = 1;
        public float HorSinFreq => horSinFreq;
        
        [Header("Bloom Tuning")]
        [SerializeField] private float diffusionRange = 0.1f;
        public float DiffusionRange => diffusionRange;
        
        [SerializeField] private float diffusionSpeed = 64f;
        public float DiffusionSpeed => diffusionSpeed;
    }
}