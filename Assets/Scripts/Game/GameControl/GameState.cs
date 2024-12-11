using System;

namespace Summoner.Game.GameControl
{
    public static class GameState
    {
        [Flags]
        private enum GameStateFlags
        {
            Gameplay = 1 << 1,
            Cutscene = 1 << 2,
            Pause = 1 << 3
        }

        public static event Action OnGameStateChanged;
        
        private static GameStateFlags stateFlags;

        public static bool InGameplay
        {
            get => HasFlag(GameStateFlags.Gameplay);
            set => SetFlag(GameStateFlags.Gameplay, value);
        }
        
        public static bool InPause
        {
            get => HasFlag(GameStateFlags.Pause);
            set => SetFlag(GameStateFlags.Pause, value);
        }

        public static bool InCutscene
        {
            get => HasFlag(GameStateFlags.Cutscene);
            set => SetFlag(GameStateFlags.Cutscene, value);
        }

        public static bool CanPause => InGameplay && !InPause;
        public static bool CanUnpause => InPause;

        private static bool HasFlag(GameStateFlags flag) => (stateFlags & flag) != 0;

        private static void SetFlag(GameStateFlags flag, bool value)
        {
            GameStateFlags previous = stateFlags;
            if (value) stateFlags.Add(flag);
            else stateFlags.Remove(flag);
            
            if (stateFlags != previous)
            {
                OnGameStateChanged?.Invoke();
            }
        }
        
        private static void Add(this ref GameStateFlags flags, GameStateFlags add) => flags |= add;
        private static void Remove(this ref GameStateFlags flags, GameStateFlags remove) => flags &= ~remove;
    }
}