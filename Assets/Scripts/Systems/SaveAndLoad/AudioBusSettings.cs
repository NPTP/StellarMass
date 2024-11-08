using System;

namespace StellarMass.Systems.SaveAndLoad
{
    [Serializable]
    public class AudioBusSettings : SaveData
    {
        public override bool ScrambleData => false;

        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;

        public override void Reset()
        {
            masterVolume = default;
            musicVolume = default;
            sfxVolume = default;
        }
    }
}