using NPTP.InputSystemWrapper.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = NPTP.InputSystemWrapper.Input;

namespace StellarMass.Game.GameControl
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menuParent;
        [SerializeField] private Transform arrowTransform;
        [SerializeField] private Transform resumeTransform;
        [SerializeField] private Transform exitTransform;
        
        private void Start()
        {
            Input.Gameplay.Pause.OnEvent += HandlePause;
            Input.PauseMenu.Navigate.OnEvent += HandleNavigate;
            Input.PauseMenu.Submit.OnEvent += HandleSubmit;
            Input.PauseMenu.Unpause.OnEvent += HandleUnpause;
        }
        
        private void OnDestroy()
        {
            Input.Gameplay.Pause.OnEvent -= HandlePause;
            Input.PauseMenu.Navigate.OnEvent -= HandleNavigate;
            Input.PauseMenu.Submit.OnEvent -= HandleSubmit;
            Input.PauseMenu.Unpause.OnEvent -= HandleUnpause;
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

            Input.Context = InputContext.Menu;
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

            // Input.CurrentContext = InputContext.Gameplay;
            GameController.ReturnToPreviousGameState();
            menuParent.SetActive(false);
        }
    }
}