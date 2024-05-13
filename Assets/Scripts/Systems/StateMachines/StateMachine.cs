using UnityEngine;

namespace StellarMass.Systems.StateMachines
{
    public class StateMachine : MonoBehaviour
    {
        public State CurrentState { get; private set; }

        private bool updating;

        public void QueueState(State state)
        {
            if (CurrentState != null)
            {
                CurrentState.End();
            }
            
            state.Begin();
            CurrentState = state;
        }

        public void Update()
        {
            if (CurrentState == null)
            {
                return;
            }
            
            State state = CurrentState.Update();
            if (state != null)
            {
                QueueState(state);
            }
        }
    }
}