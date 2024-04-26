using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement
{
    public class InputManager : MonoBehaviour, InputActions.IGameplayActions, InputActions.IPauseMenuActions
    {
        /// <summary>
        /// CreateScriptableObjectInstanceFromType is not allowed to be called from a MonoBehaviour
        /// constructor (or instance field initializer), so call new() on it in Awake or Start instead.
        /// </summary>
        private static InputActions inputActions;

        private void Awake()
        {
            inputActions = new();
            inputActions.Gameplay.AddCallbacks(this);
            inputActions.PauseMenu.AddCallbacks(this);
            inputActions.Enable();
            
            EnableGameplayMap();
        }

        private void OnDestroy()
        {
            inputActions.Gameplay.RemoveCallbacks(this);
            inputActions.PauseMenu.RemoveCallbacks(this);
            inputActions.Disable();
        }

        public static void EnableGameplayMap()
        {
            inputActions.Gameplay.Enable();
            inputActions.PauseMenu.Disable();
        }
        
        public static void EnablePauseMenuMap()
        {
            inputActions.Gameplay.Disable();
            inputActions.PauseMenu.Enable();
        }

        private static InputState GetInputState(InputAction.CallbackContext context)
        {
            if (context.started) return InputState.Started;
            else if (context.performed) return InputState.Performed;
            else return InputState.Canceled;
        }

        #region Gameplay
        
        public static event Action<InputState> OnThrustChanged;
        public void OnThrust(InputAction.CallbackContext context) => OnThrustChanged?.Invoke(GetInputState(context));

        public static event Action<InputState> OnShootChanged;
        public void OnShoot(InputAction.CallbackContext context) => OnShootChanged?.Invoke(GetInputState(context));

        public static event Action<InputState> OnHyperspaceChanged;
        public void OnHyperspace(InputAction.CallbackContext context) => OnHyperspaceChanged?.Invoke(GetInputState(context));
        public static event Action<InputState, float> OnTurnChanged;
        public void OnTurn(InputAction.CallbackContext context) => OnTurnChanged?.Invoke(GetInputState(context), context.ReadValue<float>());

        #endregion
        
        #region PauseMenu

        public static event Action<InputState> OnNavigateChanged;
        public void OnNavigate(InputAction.CallbackContext context) => OnNavigateChanged?.Invoke(GetInputState(context));

        public static event Action<InputState> OnSubmitChanged;
        public void OnSubmit(InputAction.CallbackContext context) => OnSubmitChanged?.Invoke(GetInputState(context));
        
        #endregion
    }
}