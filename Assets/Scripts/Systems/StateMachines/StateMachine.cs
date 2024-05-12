using UnityEngine;

namespace StellarMass.Systems.StateMachines
{
    public class StateMachine : MonoBehaviour
    {
        public State PreviousState { get; private set; }
        public State CurrentState { get; private set; }

        public void ChangeState(State state)
        {
            CurrentState.End();
            PreviousState = CurrentState;
            CurrentState = state;
        }
        
        public void Update()
        {
            CurrentState.Update();
        }
    }
}