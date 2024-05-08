using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement
{
    [Serializable]
    public struct InputActionReferenceWrapper
    {
        public event Action<InputAction.CallbackContext> OnAction
        {
            add => ChangeSubscription(subscribing: true, value);
            remove => ChangeSubscription(subscribing: false, value);
        }

        public bool Triggered => InputManager.GetLocalAssetActionFromReference(this).triggered;

        [SerializeField] private InputActionReference internalReference;
        public string MapName => internalReference == null ? string.Empty : internalReference.action.actionMap.name;
        public string ActionName => internalReference == null ? string.Empty : internalReference.action.name;
        
        private void ChangeSubscription(bool subscribing, Action<InputAction.CallbackContext> callback)
        {
            InputAction action = InputManager.GetLocalAssetActionFromReference(this);
            
            if (action == null)
            {
                return;
            }
            
            action.started -= callback;
            action.performed -= callback;
            action.canceled -= callback;
            
            if (!subscribing)
            {
                return;
            }
            
            action.started += callback;
            action.performed += callback;
            action.canceled += callback;
        }
        
#if UNITY_EDITOR
        public static string EDITOR_GetInternalReferenceName() => nameof(internalReference);
        
        public void EDITOR_CreateObjectField(string label)
        {
            internalReference = EditorGUILayout.ObjectField(
                label,
                internalReference,
                typeof(InputActionReference),
                allowSceneObjects: false) 
                as InputActionReference;
        }
#endif
    }
}
