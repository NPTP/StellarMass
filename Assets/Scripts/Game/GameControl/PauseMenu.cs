using NPTP.InputSystemWrapper.Enums;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.EntryExit;
using Summoner.Systems.TimeControl;
using Summoner.Utilities;
using Summoner.Utilities.Extensions;
using Summoner.Utilities.FMODUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = NPTP.InputSystemWrapper.Input;

namespace Summoner.Game.GameControl
{
    public class PauseMenu : MonoBehaviour, ITimescaleChanger
    {
        [SerializeField] private GameObject menuParent;
        [SerializeField] private Transform arrowTransform;
        [SerializeField] private PauseMenuItem resume;
        [SerializeField] private PauseMenuItem exit;

        private PauseMenuItem currentSelected;
        
        private void Awake()
        {
            currentSelected = resume;
            
            Input.Gameplay.Pause.OnEvent += HandlePauseAction;
            Input.Menu.Navigate.OnEvent += HandleNavigateAction;
            Input.Menu.Submit.OnEvent += HandleSubmitAction;
            Input.Menu.Cancel.OnEvent += HandleCancelAction;
        }
        
        private void OnDestroy()
        {
            Input.Gameplay.Pause.OnEvent -= HandlePauseAction;
            Input.Menu.Navigate.OnEvent -= HandleNavigateAction;
            Input.Menu.Submit.OnEvent -= HandleSubmitAction;
            Input.Menu.Cancel.OnEvent -= HandleCancelAction;
        }

        private void OpenPauseMenu()
        {
            TimeScaleController.RequestTimeScaleChange(this, 0);
            GameState.InPause = true;
            Input.Context = InputContext.Menu;
            menuParent.SetActive(true);
            Select(resume);
        }

        private void ClosePauseMenu()
        {
            TimeScaleController.ResetTimeScaleChanger(this);
            menuParent.SetActive(false);
            GameState.InPause = false;
        }

        private void HandlePauseAction(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started && GameState.CanPause)
            {
                OpenPauseMenu();
            }
        }

        private void HandleNavigateAction(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.started || currentSelected == null)
            {
                return;
            }

            Vector2 direction = callbackContext.ReadValue<Vector2>();
            switch (direction.ToDigitalDirection())
            {
                case Direction.Up:
                    Select(currentSelected.Up);
                    break;
                case Direction.Down:
                    Select(currentSelected.Down);
                    break;
            }
        }

        private void HandleSubmitAction(InputAction.CallbackContext callbackContext)
        {
            if (!GameState.InPause || currentSelected == null)
            {
                return;
            }

            if (currentSelected == resume)
            {
                ClosePauseMenu();
            }
            else if (currentSelected == exit)
            {
                Exit.QuitApplication();
            }
        }

        private void HandleCancelAction(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started && GameState.CanUnpause)
            {
                ClosePauseMenu();
            }
        }

        private void Select(PauseMenuItem pauseMenuItem)
        {
            if (pauseMenuItem == null)
            {
                return;
            }
            
            currentSelected = pauseMenuItem;
            arrowTransform.position = pauseMenuItem.ArrowPositionTransform.position;
            PD.Audio.Select.PlayOneShot();
        }
    }
}