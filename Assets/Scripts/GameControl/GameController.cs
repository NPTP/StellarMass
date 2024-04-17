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

        private void Start()
        {
#if !UNITY_EDITOR
            Cursor.visible = false;
#endif

            ExecuteGamePhase(startingPhaseIndex);
        }

        private void ExecuteGamePhase(int index)
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
                ExecuteGamePhase(index + 1);
            }
        }
    }
}