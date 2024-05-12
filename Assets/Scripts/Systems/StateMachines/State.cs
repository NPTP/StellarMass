namespace StellarMass.Systems.StateMachines
{
    public abstract class State
    {
        public abstract void Begin();
        public abstract void End();
        
        public virtual void Update() { }
    }
}