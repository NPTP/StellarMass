using System;
using System.Linq;

namespace Summoner.Systems.StateMachines
{
    public abstract class State
    {
        public event Action<State> OnRequestedQueueState;
        public event Action<State> OnEnded;

        /// <summary>
        /// Specify State Types that cannot be reached from this State.
        /// </summary>
        protected virtual Type[] DisallowedTransitions => Array.Empty<Type>();
        
        public bool IsTransitionDisallowed<T>() where T : State => DisallowedTransitions.Contains(typeof(T));
        public bool IsTransitionDisallowed(State state) => DisallowedTransitions.Contains(state.GetType());

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