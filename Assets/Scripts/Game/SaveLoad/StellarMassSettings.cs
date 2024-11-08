using StellarMass.Systems.SaveLoad;

namespace StellarMass.Game.SaveLoad
{
    public class StellarMassSettings : SaveData
    {
        public override bool ScrambleData => false;
        
        public override void Reset()
        {
        }
    }
}