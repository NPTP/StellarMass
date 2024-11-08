using StellarMass.Systems.SaveAndLoad;

namespace StellarMass.Game.SaveLoad
{
    public class StellarMassSettings : SaveData
    {
        public override bool ScrambleData => true;

        public override void Reset()
        {
        }
    }
}