using System;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    [Serializable]
    public class Gameplay : MapInstance, InputActions.IGameplayActions
    {
        private InputActions.GameplayActions GameplayActions { get; }
        protected override InputActionMap ActionMap { get; }

        public event Action<ActionState> @OnThrust;
        public event Action<ActionState> @OnShoot;
        public event Action<ActionState> @OnHyperspace;
        public event Action<ActionState, float> @OnTurn;
        public event Action<ActionState> @OnPause;

        public Gameplay(InputActions.GameplayActions gameplayActions)
        {
            GameplayActions = gameplayActions;
            GameplayActions.AddCallbacks(this);
            ActionMap = GameplayActions.Get();
        }

        public override void Terminate() => GameplayActions.RemoveCallbacks(this);

        void InputActions.IGameplayActions.OnThrust(InputAction.CallbackContext context) =>
            OnThrust?.Invoke(GetActionState(context));

        void InputActions.IGameplayActions.OnShoot(InputAction.CallbackContext context) =>
            OnShoot?.Invoke(GetActionState(context));

        void InputActions.IGameplayActions.OnHyperspace(InputAction.CallbackContext context) =>
            OnHyperspace?.Invoke(GetActionState(context));

        void InputActions.IGameplayActions.OnTurn(InputAction.CallbackContext context) =>
            OnTurn?.Invoke(GetActionState(context), context.ReadValue<float>());

        void InputActions.IGameplayActions.OnPause(InputAction.CallbackContext context) =>
            OnPause?.Invoke(GetActionState(context));
    }
}