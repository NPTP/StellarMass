using System;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    [Serializable]
    public abstract class MapInstance
    {
        protected InputActionMap ActionMap { private get; set; }

        public void Enable()
        {
            ActionMap.Enable();
            RemoveCallbacks();
            AddCallbacks();
        }

        public void Disable()
        {
            ActionMap.Disable();
            RemoveCallbacks();
        }

        public void Terminate() => RemoveCallbacks();

        protected abstract void AddCallbacks();
        protected abstract void RemoveCallbacks();
    }
}