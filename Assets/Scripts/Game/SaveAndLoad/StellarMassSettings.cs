using Summoner.Systems.SaveAndLoad;

namespace Summoner.Game.SaveAndLoad
{
    public class StellarMassSettings : SaveData
    {
        public override bool ScrambleData => true;

        public bool hasSeenSplashScreen;
        
        public override void Reset()
        {
            hasSeenSplashScreen = default;
        }
    }
}