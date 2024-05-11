using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement
{
    [Serializable]
    public struct InputActionReferenceWrapper
    {
        [SerializeField] private InputActionReference internalReference;

        public event Action<InputAction.CallbackContext> OnAction
        {
            add => InputManager.ChangeSubscription(internalReference, value, subscribe: true);
            remove => InputManager.ChangeSubscription(internalReference, value, subscribe: false);
        }

#if UNITY_EDITOR
        public static string EDITOR_GetInternalReferenceName() => nameof(internalReference);
#endif
    }
}
