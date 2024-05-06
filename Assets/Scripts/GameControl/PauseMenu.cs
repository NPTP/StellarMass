using StellarMass.InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.GameControl
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menuParent;
        [SerializeField] private Transform arrowTransform;
        [SerializeField] private Transform resumeTransform;
        [SerializeField] private Transform exitTransform;
        
        private void Start()
        {
            InputManager.Gameplay.OnPause += HandlePause;
            InputManager.PauseMenu.OnNavigate += HandleNavigate;
            InputManager.PauseMenu.OnSubmit += HandleSubmit;
            InputManager.PauseMenu.OnUnpause += HandleUnpause;
        }
        
        private void OnDestroy()
        {
            InputManager.Gameplay.OnPause -= HandlePause;
            InputManager.PauseMenu.OnNavigate -= HandleNavigate;
            InputManager.PauseMenu.OnSubmit -= HandleSubmit;
            InputManager.PauseMenu.OnUnpause -= HandleUnpause;
        }

        private void HandlePause(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.started)
            {
                return;
            }
            
            if (GameController.GameState != GameState.Gameplay && GameController.GameState != GameState.Cutscene)
            {
                return;
            }

            InputManager.EnableContext(InputContext.PauseMenu);
            GameController.GameState = GameState.PauseMenu;
            menuParent.SetActive(true);
            
            // NP TODO: create existing pause menu functionality
        }

        private void HandleNavigate(InputAction.CallbackContext callbackContext)
        {
        }

        private void HandleSubmit(InputAction.CallbackContext callbackContext)
        {
        }

        private void HandleUnpause(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.started || GameController.GameState != GameState.PauseMenu)
            {
                return;
            }
            
            InputManager.EnableContext(InputContext.Gameplay);
            GameController.ReturnToPreviousGameState();
            menuParent.SetActive(false);
        }
    }
}