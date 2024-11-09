using System;

namespace Summoner.Systems.SaveAndLoad
{
    /// <summary>
    /// Overwrite to create custom save data for the specific game.
    /// </summary>
    [Serializable]
    public abstract class SaveData
    {
        public int id;
        public abstract bool ScrambleData { get; }
        
        /// <summary>
        /// This should reset all fields in the save data to their defaults.
        /// E.g. id = default;
        /// </summary>
        public abstract void Reset();
    }
}