using System;
using System.Collections;
using Summoner.Systems.Coroutines;
using UnityEngine;

namespace Summoner.Game.GameControl.Phases
{
    public abstract class GamePhase : ScriptableObject
    {
        public event Action OnCompleted;

        public void Execute(GameController gameController)
        {
            CoroutineOwner.StartRoutine(ExecutionRoutine(gameController));
        }

        private IEnumerator ExecutionRoutine(GameController gameController)
        {
            yield return Execution(gameController);
            OnCompleted?.Invoke();
        }

        protected abstract IEnumerator Execution(GameController gameController);
    }
}