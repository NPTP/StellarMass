using Summoner.Game.Data;
using Summoner.Game.Data.Persistent;

namespace Summoner.Systems.Data.Persistent
{
    /// <summary>
    /// Convenient alias for PersistentData static class.
    /// </summary>
    public static class PD
    {
        public static CorePersistentData Core => PersistentData.Core;
        public static GamePersistentData Game => PersistentData.Game;
        public static PlayerPersistentData Player => PersistentData.Player;
        public static AudioPersistentData Audio => PersistentData.Audio;
        public static CameraPersistentData Camera => PersistentData.Camera;
        public static ScenesPersistentData Scenes => PersistentData.Scenes;
    }
}