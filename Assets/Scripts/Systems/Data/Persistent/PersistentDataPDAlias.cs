using StellarMass.Game.Data;
using StellarMass.Game.Data.Persistent;

namespace StellarMass.Systems.Data.Persistent
{
    /// <summary>
    /// Alias for PersistentData to make accessing members of PersistentData smaller/cleaner in code. E.g.:
    /// PersistentData.Game.DataPoint => PD.Game.DataPoint
    /// </summary>
    public static class PD
    {
        public static GamePersistentData Game => PersistentData.Game;
        public static PlayerPersistentData Player => PersistentData.Player;
        public static AudioPersistentData Audio => PersistentData.Audio;
    }
}