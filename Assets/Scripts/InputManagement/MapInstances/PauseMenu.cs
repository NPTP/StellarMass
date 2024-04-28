using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    [Serializable]
    public class PauseMenu : MapInstance, InputActionsGenerated.IPauseMenuActions
    {
        private InputActionsGenerated.PauseMenuActions PauseMenuActions { get; }

        public event Action<InputActionPhase, Vector2> @OnNavigate;
        public event Action<InputActionPhase> @OnSubmit;
        public event Action<InputActionPhase> @OnUnpause;

        public PauseMenu(InputActionsGenerated.PauseMenuActions pauseMenuActions)
        {
            PauseMenuActions = pauseMenuActions;
            ActionMap = PauseMenuActions.Get();
            AddCallbacks();
        }

        protected sealed override void AddCallbacks() => PauseMenuActions.AddCallbacks(this);
        protected sealed override void RemoveCallbacks() => PauseMenuActions.RemoveCallbacks(this);

        void InputActionsGenerated.IPauseMenuActions.OnNavigate(InputAction.CallbackContext context) =>
            OnNavigate?.Invoke(context.phase, context.ReadValue<Vector2>());

        void InputActionsGenerated.IPauseMenuActions.OnSubmit(InputAction.CallbackContext context) =>
            OnSubmit?.Invoke(context.phase);

        void InputActionsGenerated.IPauseMenuActions.OnUnpause(InputAction.CallbackContext context) =>
            OnUnpause?.Invoke(context.phase);
    }
}