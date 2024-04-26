using StellarMass.OldInput;
using UnityEngine;

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
            InputReceiver.AddListeners(InputType.Pause, HandlePause);
            
            InputReceiver.AddListeners(InputType.Unpause, HandleUnpause);
            InputReceiver.AddListeners(InputType.MenuUp, HandleUp);
            InputReceiver.AddListeners(InputType.MenuDown, HandleDown);
        }

        private void OnDestroy()
        {
            InputReceiver.RemoveListeners(InputType.Pause, HandlePause);
            
            InputReceiver.RemoveListeners(InputType.Unpause, HandleUnpause);
            InputReceiver.RemoveListeners(InputType.MenuUp, HandleUp);
            InputReceiver.RemoveListeners(InputType.MenuDown, HandleDown);
        }

        private void HandlePause()
        {
            if (GameController.GameState != GameState.Gameplay && GameController.GameState != GameState.Cutscene)
            {
                return;
            }

            InputReceiver.ActiveInputMap = InputMap.Menu;
            GameController.GameState = GameState.PauseMenu;
            menuParent.SetActive(true);
            
            // NP TODO: create existing pause menu functionality
        }

        private void HandleUnpause()
        {
            if (GameController.GameState != GameState.PauseMenu)
            {
                return;
            }

            InputReceiver.ActiveInputMap = InputMap.Gameplay;
            GameController.ReturnToPreviousGameState();
            menuParent.SetActive(false);
        }
        
        private void HandleUp()
        {
            Debug.Log(nameof(HandleUp));
        }

        private void HandleDown()
        {
            Debug.Log(nameof(HandleDown));
        }
    }
}