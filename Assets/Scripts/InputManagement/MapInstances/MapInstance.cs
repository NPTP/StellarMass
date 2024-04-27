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

        protected static ActionState GetActionState(InputAction.CallbackContext context)
        {
            if (context.started) return ActionState.Started;
            else if (context.performed) return ActionState.Performed;
            else return ActionState.Canceled;
        }

        protected abstract void AddCallbacks();
        protected abstract void RemoveCallbacks();
    }
}