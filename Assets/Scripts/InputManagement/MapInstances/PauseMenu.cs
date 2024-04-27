using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    [Serializable]
    public class PauseMenu : MapInstance, InputActions.IPauseMenuActions
    {
        private InputActions.PauseMenuActions PauseMenuActions { get; }

        public event Action<ActionState, Vector2> @OnNavigate;
        public event Action<ActionState> @OnSubmit;
        public event Action<ActionState> @OnUnpause;

        public PauseMenu(InputActions.PauseMenuActions pauseMenuActions)
        {
            PauseMenuActions = pauseMenuActions;
            ActionMap = PauseMenuActions.Get();
            AddCallbacks();
        }

        protected sealed override void AddCallbacks() => PauseMenuActions.AddCallbacks(this);
        protected sealed override void RemoveCallbacks() => PauseMenuActions.RemoveCallbacks(this);

        void InputActions.IPauseMenuActions.OnNavigate(InputAction.CallbackContext context) =>
            OnNavigate?.Invoke(GetActionState(context), context.ReadValue<Vector2>());

        void InputActions.IPauseMenuActions.OnSubmit(InputAction.CallbackContext context) =>
            OnSubmit?.Invoke(GetActionState(context));

        void InputActions.IPauseMenuActions.OnUnpause(InputAction.CallbackContext context) =>
            OnUnpause?.Invoke(GetActionState(context));
    }
}