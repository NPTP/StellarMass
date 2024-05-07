using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement
{
    [Serializable]
    public class InputActionReferenceWrapped
    {
        public event Action<InputAction.CallbackContext> OnAction
        {
            add => ChangeSubscription(subscribing: true, value);
            remove => ChangeSubscription(subscribing: false, value);
        }

        [SerializeField] private InputActionReference reference;
        public string MapName => reference.action.actionMap.name;
        public string ActionName => reference.action.name;
        
        private void ChangeSubscription(bool subscribing, Action<InputAction.CallbackContext> callback)
        {
            InputAction action = InputManager.GetLocalAssetActionFromReference(this);
            if (action == null) return;
            if (subscribing)
            {
                action.started += callback;
                action.performed += callback;
                action.canceled += callback;
            }
            else
            {
                action.started -= callback;
                action.performed -= callback;
                action.canceled -= callback;
            }
        }
    }
}
