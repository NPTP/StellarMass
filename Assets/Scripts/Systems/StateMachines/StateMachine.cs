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

        public bool CurrentStateIs<T>() where T : State
        {
            return currentState != null && currentState.GetType() == typeof(T);
        }
        
        public bool CurrentStateIsNot<T>() where T : State
        {
            return !CurrentStateIs<T>();
        }

        public bool CurrentStateIsNot<T1, T2>() where T1 : State where T2 : State
        {
            return !CurrentStateIs<T1>() && !CurrentStateIs<T2>();
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