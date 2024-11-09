using UnityEngine;
using Summoner.Game.GameControl.Phases;
using Summoner.Systems.AudioSystem;
using Summoner.Systems.Data.Persistent;

namespace Summoner.Game.GameControl
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        public GameObject Player => player;
        [SerializeField] private MainDisplay mainDisplay;
        public MainDisplay MainDisplay => mainDisplay;

        [Space]
        [SerializeField] private bool playMusicOnStart;
        [SerializeField] private bool playAmbienceOnStart;

        [Space]
        [SerializeField] private GamePhase[] gamePhases;
        [SerializeField] private int startingPhaseIndex = 0;
        [SerializeField] private bool executeGamePhases = true;

        private static PersistentAudio music;
        private static PersistentAudio ambience;
        
        private static GameState previousGameState = GameState.Undefined;
        private static GameState gameState = GameState.Undefined;
        public static GameState GameState
        {
            get => gameState;
            set
            {
                previousGameState = gameState;
                gameState = value;
            }
        }

        private void Start()
        {
            if (playMusicOnStart) music = PersistentAudioPlayer.PlayPersistentAudio(PersistentData.Audio.Music);
            if (playAmbienceOnStart) ambience = PersistentAudioPlayer.PlayPersistentAudio(PersistentData.Audio.Ambience);
            
            ExecuteGamePhases(startingPhaseIndex);
        }

        private void ExecuteGamePhases(int index)
        {
            if (!executeGamePhases || index >= gamePhases.Length)
            {
                return;
            }
            
            GamePhase phase = gamePhases[index];
            phase.OnCompleted += handlePhaseCompleted;
            phase.Execute(this);

            void handlePhaseCompleted()
            {
                phase.OnCompleted -= handlePhaseCompleted;
                ExecuteGamePhases(index + 1);
            }
        }

        public static void ReturnToPreviousGameState()
        {
            GameState = previousGameState;
        }
    }
}