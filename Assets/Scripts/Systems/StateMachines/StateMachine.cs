using UnityEngine;

namespace Summoner.Systems.StateMachines
{
    public class StateMachine : MonoBehaviour
    {
        private State currentState;
        
        private void Update()
        {
            if (currentState != null && currentState.UpdateState(out State next))
            {
                Queue(next);
            }
        }
        
        public void Queue(State state)
        {
            currentState?.End();

            currentState = state;
            currentState.OnEnded += HandleStateEnded;
            currentState.BeginState();
        }

        private void HandleStateEnded(State state)
        {
            state.OnEnded -= HandleStateEnded;
            if (state == currentState)
            {
                currentState = null;
            }
        }
    }
}