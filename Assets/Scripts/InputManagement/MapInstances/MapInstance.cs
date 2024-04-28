using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    [Serializable]
    public abstract class MapInstance
    {
        public event Action<MapInstance> OnMapEnabled;
        protected InputActionMap ActionMap { private get; set; }

        public bool ActionMapEnabled => ActionMap.enabled;

        [SerializeField] private EventSystemActions eventSystemActions;
        public EventSystemActions EventSystemActions => eventSystemActions;

        public void Enable()
        {
            ActionMap.Enable();
            OnMapEnabled?.Invoke(this);
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