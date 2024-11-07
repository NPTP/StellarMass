using System;

namespace StellarMass.Systems.SaveLoad
{
    /// <summary>
    /// Overwrite to create custom save data for the specific game.
    /// </summary>
    [Serializable]
    public abstract class SaveData
    {
        public int id;
    }
}