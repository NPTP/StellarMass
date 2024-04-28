using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    // MARKER.Ignore.Start
    /// <summary>
    /// This script is used by the map instance code generator as a template to generate new C# input classes
    /// and their respective .cs files. Do not modify it unless you know what you're doing!
    /// (Also, don't instantiate it in your code! It will throw an exception if you try.)
    /// </summary>
    // MARKER.Ignore.End
    // MARKER.ClassName.Start
    [Serializable]
    public class MapInstanceTemplate : MapInstance,
    // MARKER.ClassName.End
    // MARKER.InterfaceName.Start
    InputActionsGenerated.IGameplayActions
    // MARKER.InterfaceName.End
    {
        // MARKER.ActionsGetterProperty.Start
        private InputActionsGenerated.GameplayActions GameplayActions { get; }
        // MARKER.ActionsGetterProperty.End

        // MARKER.PublicEvents.Start
        public event Action<InputActionPhase> @OnThrust;
        public event Action<InputActionPhase> @OnShoot;
        public event Action<InputActionPhase> @OnHyperspace;
        public event Action<InputActionPhase, float> @OnTurn;
        public event Action<InputActionPhase> @OnPause;
        // MARKER.PublicEvents.End

        // MARKER.ConstructorSignature.Start
        public MapInstanceTemplate(InputActionsGenerated.GameplayActions gameplayActions)
        // MARKER.ConstructorSignature.End
        {
            // MARKER.Ignore.Start
            throw new NotSupportedException("This class is designed as a template only. Never instantiate it!");
#pragma warning disable CS0162
            // ReSharper disable once HeuristicUnreachableCode
            // MARKER.Ignore.End
            // MARKER.SetUpActions.Start
            GameplayActions = gameplayActions;
            ActionMap = GameplayActions.Get();
            // MARKER.SetUpActions.End
            AddCallbacks();
            // MARKER.Ignore.Start
#pragma warning restore CS0162
            // MARKER.Ignore.End
        }

        protected sealed override void AddCallbacks() => 
            // MARKER.ActionsPropertyName.Start
            GameplayActions
            // MARKER.ActionsPropertyName.End
                .AddCallbacks(this);
        protected sealed override void RemoveCallbacks() => 
            // MARKER.ActionsPropertyName.Start
            GameplayActions
            // MARKER.ActionsPropertyName.End
                .RemoveCallbacks(this);

        // MARKER.InterfaceMethods.Start
        void InputActionsGenerated.IGameplayActions.OnThrust(InputAction.CallbackContext context) => OnThrust?.Invoke(context.phase);
        void InputActionsGenerated.IGameplayActions.OnShoot(InputAction.CallbackContext context) => OnShoot?.Invoke(context.phase);
        void InputActionsGenerated.IGameplayActions.OnHyperspace(InputAction.CallbackContext context) => OnHyperspace?.Invoke(context.phase);
        void InputActionsGenerated.IGameplayActions.OnTurn(InputAction.CallbackContext context) => OnTurn?.Invoke(context.phase, context.ReadValue<float>());
        void InputActionsGenerated.IGameplayActions.OnPause(InputAction.CallbackContext context) => OnPause?.Invoke(context.phase);
        // MARKER.InterfaceMethods.End
    }
}
