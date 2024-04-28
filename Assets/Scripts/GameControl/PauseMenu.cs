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

        private void HandlePause(InputActionPhase actionState)
        {
            if (actionState is not InputActionPhase.Started) return;
            
            if (GameController.GameState != GameState.Gameplay && GameController.GameState != GameState.Cutscene)
            {
                return;
            }

            InputManager.PauseMenu.Enable();
            GameController.GameState = GameState.PauseMenu;
            menuParent.SetActive(true);
            
            // NP TODO: create existing pause menu functionality
        }

        private void HandleNavigate(InputActionPhase actionState, Vector2 direction)
        {
        }

        private void HandleSubmit(InputActionPhase actionState)
        {
        }

        private void HandleUnpause(InputActionPhase actionState)
        {
            if (actionState is not InputActionPhase.Started) return;
            
            if (GameController.GameState != GameState.PauseMenu)
            {
                return;
            }

            InputManager.Gameplay.Enable();
            GameController.ReturnToPreviousGameState();
            menuParent.SetActive(false);
        }
    }
}