using System;
using System.Collections;
using Summoner.Systems.Coroutines;
using UnityEngine;

namespace Summoner.Game.GameControl.Phases
{
    public abstract class GamePhase : ScriptableObject
    {
        [SerializeField] private GameState phaseState;
        
        public event Action OnCompleted;

        public void Execute(GameController gameController)
        {
            GameController.GameState = phaseState;
            CoroutineOwner.StartRoutine(ExecutionRoutine(gameController));
        }

        private IEnumerator ExecutionRoutine(GameController gameController)
        {
            yield return Execution(gameController);
            GameController.ReturnToPreviousGameState();
            OnCompleted?.Invoke();
        }

        protected abstract IEnumerator Execution(GameController gameController);
    }
}