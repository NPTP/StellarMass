using System;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    [Serializable]
    public class Gameplay : MapInstance, InputActionsGenerated.IGameplayActions
    {
        private InputActionsGenerated.GameplayActions GameplayActions { get; }

        public event Action<InputActionPhase> @OnThrust;
        public event Action<InputActionPhase> @OnShoot;
        public event Action<InputActionPhase> @OnHyperspace;
        public event Action<InputActionPhase, float> @OnTurn;
        public event Action<InputActionPhase> @OnPause;

        public Gameplay(InputActionsGenerated.GameplayActions gameplayActions)
        {
            GameplayActions = gameplayActions;
            ActionMap = GameplayActions.Get();
            AddCallbacks();
        }

        protected sealed override void AddCallbacks() => GameplayActions.AddCallbacks(this);
        protected sealed override void RemoveCallbacks() => GameplayActions.RemoveCallbacks(this);

        void InputActionsGenerated.IGameplayActions.OnThrust(InputAction.CallbackContext context) =>
            OnThrust?.Invoke(context.phase);

        void InputActionsGenerated.IGameplayActions.OnShoot(InputAction.CallbackContext context) =>
            OnShoot?.Invoke(context.phase);

        void InputActionsGenerated.IGameplayActions.OnHyperspace(InputAction.CallbackContext context) =>
            OnHyperspace?.Invoke(context.phase);

        void InputActionsGenerated.IGameplayActions.OnTurn(InputAction.CallbackContext context) =>
            OnTurn?.Invoke(context.phase, context.ReadValue<float>());

        void InputActionsGenerated.IGameplayActions.OnPause(InputAction.CallbackContext context) =>
            OnPause?.Invoke(context.phase);
    }
}