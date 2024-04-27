using System;
using StellarMass.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    [Serializable]
    public abstract class MapInstance
    {
        public event Action<MapInstance> OnMapEnabled;
        protected abstract InputActionMap ActionMap { get; }
        public abstract void Terminate();

        public bool ActionMapEnabled => ActionMap.enabled;

        [SerializeField] private EventSystemActions eventSystemActions;
        public EventSystemActions EventSystemActions => eventSystemActions;

        public void Enable()
        {
            ActionMap.Enable();
            OnMapEnabled?.Invoke(this);
        }

        public void Disable()
        {
            ActionMap.Disable();
        }

        protected static ActionState GetActionState(InputAction.CallbackContext context)
        {
            if (context.started) return ActionState.Started;
            else if (context.performed) return ActionState.Performed;
            else return ActionState.Canceled;
        }
    }
}