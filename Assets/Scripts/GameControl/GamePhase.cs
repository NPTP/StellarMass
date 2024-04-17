using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace StellarMass.GameControl
{
    public abstract class GamePhase : ScriptableObject
    {
        public event Action OnCompleted;

        public void Execute(GameController gameController)
        {
            CoroutineOwner.StartRoutine(ExecutionRoutine(gameController));
        }

        IEnumerator ExecutionRoutine(GameController gameController)
        {
            yield return Execution(gameController);
            OnCompleted?.Invoke();
        }

        protected abstract IEnumerator Execution(GameController gameController);
    }
}