using StellarMass.GameControl.Phases;
using StellarMass.Input;
using UnityEngine;

namespace StellarMass.GameControl
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        public GameObject Player => player;
        [SerializeField] private MainDisplay mainDisplay;
        public MainDisplay MainDisplay => mainDisplay;
        [Space]
        [SerializeField] private GamePhase[] gamePhases;
        [SerializeField] private int startingPhaseIndex = 0;
        [SerializeField] private bool executeGamePhases = true;

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

        private void Awake()
        {
            InputReceiver.Initialize();
        }

        private void Start()
        {
#if !UNITY_EDITOR
            Cursor.visible = false;
#endif

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