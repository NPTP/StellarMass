using NPTP.InputSystemWrapper.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using Button = UnityEngine.InputSystem.HID.HID.Button;

// ------------------------------------------------------------------------------------------
// This file was automatically generated by InputScriptGenerator. Do not modify it manually.
// ------------------------------------------------------------------------------------------
namespace NPTP.InputSystemWrapper.Generated.Actions
{
    public class MenuActions
    {
        internal InputActionMap ActionMap { get; }
        
        public ValueActionWrapper<Vector2> Navigate { get; }
        public ActionWrapper Submit { get; }
        public ActionWrapper Cancel { get; }
        public ActionWrapper Unpause { get; }
        
        private bool enabled;
        
        internal MenuActions(InputActionAsset asset)
        {
            ActionMap = asset.FindActionMap("Menu", throwIfNotFound: true);
            
            Navigate = new (ActionMap.FindAction("Navigate", throwIfNotFound: true));
            Submit = new (ActionMap.FindAction("Submit", throwIfNotFound: true));
            Cancel = new (ActionMap.FindAction("Cancel", throwIfNotFound: true));
            Unpause = new (ActionMap.FindAction("Unpause", throwIfNotFound: true));
        }
        
        internal void EnableAndRegisterCallbacks()
        {
            if (enabled)
            {
                return;
            }

            enabled = true;
            ActionMap.Enable();
            
            Navigate.RegisterCallbacks();
            Submit.RegisterCallbacks();
            Cancel.RegisterCallbacks();
            Unpause.RegisterCallbacks();
        }
        
        internal void DisableAndUnregisterCallbacks()
        {
            if (!enabled)
            {
                return;
            }

            enabled = false;
            ActionMap.Disable();

            Navigate.UnregisterCallbacks();
            Submit.UnregisterCallbacks();
            Cancel.UnregisterCallbacks();
            Unpause.UnregisterCallbacks();
        }
    }
}