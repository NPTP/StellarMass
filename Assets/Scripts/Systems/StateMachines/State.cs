using System;

namespace Summoner.Systems.StateMachines
{
    public abstract class State
    {
        public event Action<State> OnEnded;
        
        public virtual void BeginState() { }

        public virtual bool UpdateState(out State next)
        {
            next = null;
            return false;
        }

        public void End()
        {
            EndState();
            OnEnded?.Invoke(this);
        }

        protected virtual void EndState() { }
    }
}