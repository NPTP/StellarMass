using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    [Serializable]
    public class PauseMenu : MapInstance, InputActions.IPauseMenuActions
    {
        private InputActions.PauseMenuActions PauseMenuActions { get; }
        protected override InputActionMap ActionMap { get; }

        public event Action<ActionState, Vector2> @OnNavigate;
        public event Action<ActionState> @OnSubmit;
        public event Action<ActionState> @OnUnpause;

        public PauseMenu(InputActions.PauseMenuActions pauseMenuActions)
        {
            PauseMenuActions = pauseMenuActions;
            PauseMenuActions.AddCallbacks(this);
            ActionMap = PauseMenuActions.Get();
        }

        public override void Terminate() => PauseMenuActions.RemoveCallbacks(this);

        void InputActions.IPauseMenuActions.OnNavigate(InputAction.CallbackContext context) =>
            OnNavigate?.Invoke(GetActionState(context), context.ReadValue<Vector2>());

        void InputActions.IPauseMenuActions.OnSubmit(InputAction.CallbackContext context) =>
            OnSubmit?.Invoke(GetActionState(context));

        void InputActions.IPauseMenuActions.OnUnpause(InputAction.CallbackContext context) =>
            OnUnpause?.Invoke(GetActionState(context));
    }
}