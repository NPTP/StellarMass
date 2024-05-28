using StellarMass.Game.Data;
using StellarMass.Game.GameControl.Phases;
using StellarMass.Systems.AudioSystem;
using UnityEngine;

namespace StellarMass.Game.GameControl
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
#if !UNITY_EDITOR
            Cursor.visible = false;
#endif
            
            if (playMusicOnStart) music = AudioPlayer.PlayPersistentAudio(AudioData.Music);
            if (playAmbienceOnStart) ambience = AudioPlayer.PlayPersistentAudio(AudioData.Ambience);
            
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