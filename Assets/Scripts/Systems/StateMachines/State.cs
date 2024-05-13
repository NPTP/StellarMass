using System;

namespace StellarMass.Systems.StateMachines
{
    public abstract class State
    {
        public virtual void Begin() { }

        public virtual State Update()
        {
            return null;
        }

        public virtual void End() { }
    }
}