using StellarMass.Systems.Data;

namespace StellarMass.Systems.Initialization
{
    public abstract class ExecutableStep : DataScriptable
    {
        public abstract void Execute();
    }
}