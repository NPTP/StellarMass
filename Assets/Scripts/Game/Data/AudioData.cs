using StellarMass.Systems.Audio;
using StellarMass.Systems.Data;
using UnityEngine;

namespace StellarMass.Game.Data
{
    [CreateAssetMenu]
    public class AudioData : DataScriptable
    {
        [SerializeField] private Sound music;
        public Sound Music => music;

        [SerializeField] private Sound ambience;
        public Sound Ambience => ambience;
    }
}