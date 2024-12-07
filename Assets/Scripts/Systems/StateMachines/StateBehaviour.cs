using System;
using UnityEngine;

namespace Summoner.Systems.StateMachines
{
    public abstract class StateBehaviour : ScriptableObject
    {
        protected const string CREATE_ASSET_PATH = "StateBehaviours/";
        
        public abstract Type StateInstanceType { get; }
    }
    
    public abstract class StateBehaviour<T> : StateBehaviour where T : StateInstance
    {
        public sealed override Type StateInstanceType => typeof(T);

        public virtual void Begin(T input) { }

        public virtual bool UpdateState(T input, out StateInstance next)
        {
            next = null;
            return false;
        }

        public virtual void End(T input) { }
    }
}