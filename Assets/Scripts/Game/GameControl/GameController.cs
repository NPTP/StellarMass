using UnityEngine;
using Summoner.Game.GameControl.Phases;
using Summoner.Systems.AudioSystem;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.MonoReferences;

namespace Summoner.Game.GameControl
{
    public class GameController : ReferenceTableMonoBehaviour<GameController>
    {
        [SerializeField] private GameObject player;
        public GameObject Player => player;
        [SerializeField] private MainDisplay mainDisplay;
        public MainDisplay MainDisplay => mainDisplay;
        [SerializeField] private Transform spawnedObjectsParent;
        public Transform SpawnedObjectsParent => spawnedObjectsParent;

        [Space]
        [SerializeField] private bool playAmbienceOnStart;

        [Space]
        [SerializeField] private GamePhase[] gamePhases;
        [SerializeField] private int startingPhaseIndex = 0;
        [SerializeField] private bool executeGamePhases = true;

        private static PersistentAudio music;
        private static PersistentAudio ambience;

        private void Start()
        {
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
    }
}