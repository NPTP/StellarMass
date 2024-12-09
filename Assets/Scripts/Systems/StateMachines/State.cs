using System;

namespace Summoner.Systems.StateMachines
{
    public abstract class State
    {
        public event Action<State> OnRequestedQueueState;
        public event Action<State> OnEnded;

        public virtual void BeginState() { }

        public virtual void UpdateState() { }
        
        public virtual void FixedUpdateState() { }

        public void End()
        {
            EndState();
            OnEnded?.Invoke(this);
        }

        protected virtual void EndState() { }

        protected void Queue(State state)
        {
            OnRequestedQueueState?.Invoke(state);
        }
    }
}