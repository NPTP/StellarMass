using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement
{
    [Serializable]
    public class ActionReferenceWrapper
    {
        public event Action<InputAction.CallbackContext> OnAction
        {
            add
            {
                InputAction action = InputManager.GetLocalAssetActionFromReference(this);
                action.started += value;
                action.performed += value;
                action.canceled += value;
            }
            remove
            {
                InputAction action = InputManager.GetLocalAssetActionFromReference(this);
                action.started -= value;
                action.performed -= value;
                action.canceled -= value;
            }
        }
        
        [SerializeField] private InputActionReference reference;
        public string MapName => reference.action.actionMap.name;
        public string ActionName => reference.action.name;
    }
}
