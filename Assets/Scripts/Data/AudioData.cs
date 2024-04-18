using StellarMass.Audio;
using UnityEngine;

namespace StellarMass.Data
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