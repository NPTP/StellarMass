﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    // MARKER.Ignore.Start
    /// <summary>
    /// This script is used by the map instance code generator as a template to generate new C# input classes
    /// and their respective .cs files. Do not modify it unless you know what you're doing!
    /// (Also, don't instantiate it in your code - it will throw an exception if you try.)
    /// </summary>
    // MARKER.Ignore.End
    // MARKER.ClassSignature.Start
    [Serializable]
    public class MapInstanceTemplate : MapInstance, InputActions.IGameplayActions
    // MARKER.ClassSignature.End
    {
        // MARKER.ActionsGetterProperty.Start
        private InputActions.GameplayActions GameplayActions { get; }
        // MARKER.ActionsGetterProperty.End

        // MARKER.PublicEvents.Start
        public event Action<InputActionPhase> @OnThrust;
        public event Action<InputActionPhase> @OnShoot;
        public event Action<InputActionPhase> @OnHyperspace;
        public event Action<InputActionPhase, float> @OnTurn;
        public event Action<InputActionPhase> @OnPause;
        // MARKER.PublicEvents.End

        // MARKER.ConstructorSignature.Start
        public MapInstanceTemplate(InputActions.GameplayActions gameplayActions)
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

        // MARKER.AddCallbacks.Start
        protected sealed override void AddCallbacks() => GameplayActions.AddCallbacks(this);
        // MARKER.AddCallbacks.End
        // MARKER.RemoveCallbacks.Start
        protected sealed override void RemoveCallbacks() => GameplayActions.RemoveCallbacks(this);
        // MARKER.RemoveCallbacks.End

        // MARKER.InterfaceMethods.Start
        void InputActions.IGameplayActions.OnThrust(InputAction.CallbackContext context) => OnThrust?.Invoke(context.phase);
        void InputActions.IGameplayActions.OnShoot(InputAction.CallbackContext context) => OnShoot?.Invoke(context.phase);
        void InputActions.IGameplayActions.OnHyperspace(InputAction.CallbackContext context) => OnHyperspace?.Invoke(context.phase);
        void InputActions.IGameplayActions.OnTurn(InputAction.CallbackContext context) => OnTurn?.Invoke(context.phase, context.ReadValue<float>());
        void InputActions.IGameplayActions.OnPause(InputAction.CallbackContext context) => OnPause?.Invoke(context.phase);
        // MARKER.InterfaceMethods.End
    }
}