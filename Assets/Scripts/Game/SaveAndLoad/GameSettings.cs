using Summoner.Systems.SaveAndLoad;

namespace Summoner.Game.SaveAndLoad
{
    public class GameSettings : SaveData
    {
        public override bool ScrambleData => false;

        public float masterVolume = 1.0f;
        public float musicVolume = 1.0f;
        public float sfxVolume = 1.0f;

        public bool hasSeenSplashScreen = false;
        
        public override void Reset()
        {
            masterVolume = default;
            musicVolume = default;
            sfxVolume = default;
            
            hasSeenSplashScreen = default;
        }
    }
}