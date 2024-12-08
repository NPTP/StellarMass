using System;
using UnityEngine;

namespace Summoner.Systems.StateMachines
{
    public class StateMachine : MonoBehaviour
    {
        private State currentState;
        
        private void Update()
        {
            currentState?.UpdateState();
        }

        private void FixedUpdate()
        {
            currentState?.FixedUpdateState();
        }

        public void Queue(State state)
        {
            currentState?.End();

            currentState = state;
            currentState.OnRequestedQueueState += HandleRequestedQueueState;
            currentState.OnEnded += HandleStateEnded;
            currentState.BeginState();
        }

        private void HandleRequestedQueueState(State queueState)
        {
            currentState.OnRequestedQueueState -= HandleRequestedQueueState;
            Queue(queueState);
        }

        private void HandleStateEnded(State state)
        {
            currentState.OnRequestedQueueState -= HandleRequestedQueueState;
            state.OnEnded -= HandleStateEnded;
            if (state == currentState)
            {
                currentState = null;
            }
        }
    }
}