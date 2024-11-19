using System;

namespace Summoner.Game.GameControl
{
    public static class GameState
    {
        [Flags]
        public enum GameStateFlags
        {
            Gameplay = 1 << 1,
            Cutscene = 1 << 2,
            Pause = 1 << 3
        }

        private static GameStateFlags stateFlags;

        public static event Action OnGameStateChanged;
        
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

        private static bool HasFlag(GameStateFlags flag) => stateFlags.HasFlag(flag);

        private static void SetFlag(GameStateFlags flag, bool value)
        {
            GameStateFlags previous = stateFlags;
            if (value) stateFlags |= flag;
            else stateFlags &= ~flag;
            
            if (stateFlags != previous)
            {
                OnGameStateChanged?.Invoke();
            }
        }
    }

    public static class FlagsExtensions
    {
        public static void AddFlag(this GameState.GameStateFlags flags, GameState.GameStateFlags add)
        {
            flags |= add;
        }

        public static void RemoveFlag(this GameState.GameStateFlags flags, GameState.GameStateFlags remove)
        {
            flags &= ~remove;
        }
    }
}