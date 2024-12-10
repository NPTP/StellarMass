using UnityEngine;

namespace Summoner.Systems.StateMachines
{
    public class StateMachine : MonoBehaviour
    {
        public State CurrentState { get; private set; }

        private void Update()
        {
            CurrentState?.UpdateState();
        }

        private void FixedUpdate()
        {
            CurrentState?.FixedUpdateState();
        }

        public bool CurrentStateIs<TState>() where TState : State
        {
            return CurrentState != null && CurrentState.GetType() == typeof(TState);
        }
        
        public bool CurrentStateIsNot<TState>() where TState : State
        {
            return !CurrentStateIs<TState>();
        }
        
        public void Queue(State nextState)
        {
            if (CurrentState != null)
            {
                if (CurrentState.IsTransitionDisallowed(nextState))
                {
                    return;
                }
                
                CurrentState.End();
            }
            
            CurrentState = nextState;
            CurrentState.OnRequestedQueueState += HandleRequestedQueueState;
            CurrentState.OnEnded += HandleStateEnded;
            CurrentState.BeginState();
        }

        private void HandleRequestedQueueState(State queueState)
        {
            CurrentState.OnRequestedQueueState -= HandleRequestedQueueState;
            Queue(queueState);
        }

        private void HandleStateEnded(State state)
        {
            CurrentState.OnRequestedQueueState -= HandleRequestedQueueState;
            state.OnEnded -= HandleStateEnded;
            if (state == CurrentState)
            {
                CurrentState = null;
            }
        }
    }
}