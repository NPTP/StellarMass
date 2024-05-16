using System;
using System.Collections.Generic;
using StellarMass.Systems.InputManagement.Generated.MapActions;
using StellarMass.Systems.InputManagement.Generated.MapCaches;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Object = UnityEngine.Object;

namespace StellarMass.Systems.InputManagement
{
    public class InputPlayer
    {
        #region Field & Properties
        
        public event Action<ControlScheme> OnControlSchemeChanged;
        public event Action<char> OnKeyboardTextInput;

        public int ID { get; }
        public InputContext CurrentContext { get; private set; }
        public ControlScheme CurrentControlScheme { get; private set; }
        public PlayerInput PlayerInput
        {
            set
            {
                if (playerInput == value) return;
                if (playerInput != null) playerInput.onControlsChanged -= HandleControlsChanged;
                playerInput = value;
                if (playerInput != null) playerInput.onControlsChanged += HandleControlsChanged;
            }
        }
        public InputSystemUIInputModule UIInputModule
        {
            set
            {
                uiInputModule = value;
                if (playerInput != null) playerInput.uiInputModule = value;
            }
        }
        
        // MARKER.MapActionsProperties.Start
        public GameplayActions Gameplay { get; }
        public PauseMenuActions PauseMenu { get; }
        // MARKER.MapActionsProperties.End
        
        // MARKER.MapCacheFields.Start
        private readonly GameplayMapCache gameplayMap;
        private readonly PauseMenuMapCache pauseMenuMap;
        // MARKER.MapCacheFields.End
        
        private readonly InputActionAsset asset;
        private PlayerInput playerInput;
        private InputSystemUIInputModule uiInputModule;
        
        #endregion

        public InputPlayer(InputActionAsset asset, int id)
        {
            this.asset = asset;
            ID = id;

            // MARKER.MapAndActionsInstantiation.Start
            Gameplay = new GameplayActions();
            gameplayMap = new GameplayMapCache(asset);
            PauseMenu = new PauseMenuActions();
            pauseMenuMap = new PauseMenuMapCache(asset);
            // MARKER.MapAndActionsInstantiation.End
        }

        #region Public Interface
        
        public void Terminate()
        {
            asset.Disable();
            DisableKeyboardTextInput();
            RemoveAllMapActionCallbacks();
            Object.Destroy(playerInput);
            Object.Destroy(uiInputModule);
        }
        
        public void EnableContext(InputContext context)
        {
            CurrentContext = context;
            EnableMapsForContext(context);
        }
        
        public void FindActionEventAndSubscribe(InputActionReference actionReference, Action<InputAction.CallbackContext> callback, bool subscribe)
        {
            InputActionMap map = asset.FindActionMap(actionReference.action.actionMap.name);
            if (map == null) return;
            InputAction action = map.FindAction(actionReference.action.name);
            if (action == null) return;

            // MARKER.ChangeSubscriptionIfStatements.Start
            if (gameplayMap.ActionMap == map)
            {
                if (action == gameplayMap.Thrust)
                {
                    Gameplay.OnThrust -= callback;
                    if (subscribe) Gameplay.OnThrust += callback;
                }
                else if (action == gameplayMap.Shoot)
                {
                    Gameplay.OnShoot -= callback;
                    if (subscribe) Gameplay.OnShoot += callback;
                }
                else if (action == gameplayMap.Hyperspace)
                {
                    Gameplay.OnHyperspace -= callback;
                    if (subscribe) Gameplay.OnHyperspace += callback;
                }
                else if (action == gameplayMap.Turn)
                {
                    Gameplay.OnTurn -= callback;
                    if (subscribe) Gameplay.OnTurn += callback;
                }
                else if (action == gameplayMap.Pause)
                {
                    Gameplay.OnPause -= callback;
                    if (subscribe) Gameplay.OnPause += callback;
                }
            }
            else if (pauseMenuMap.ActionMap == map)
            {
                if (action == pauseMenuMap.Navigate)
                {
                    PauseMenu.OnNavigate -= callback;
                    if (subscribe) PauseMenu.OnNavigate += callback;
                }
                else if (action == pauseMenuMap.Submit)
                {
                    PauseMenu.OnSubmit -= callback;
                    if (subscribe) PauseMenu.OnSubmit += callback;
                }
                else if (action == pauseMenuMap.Unpause)
                {
                    PauseMenu.OnUnpause -= callback;
                    if (subscribe) PauseMenu.OnUnpause += callback;
                }
            }
            // MARKER.ChangeSubscriptionIfStatements.End
        }
        
        #endregion

        #region Private Functionality

        private void RemoveAllMapActionCallbacks()
        {
            // MARKER.MapActionsRemoveCallbacks.Start
            gameplayMap.RemoveCallbacks(Gameplay);
            pauseMenuMap.RemoveCallbacks(PauseMenu);
            // MARKER.MapActionsRemoveCallbacks.End
        }

        private void EnableKeyboardTextInput()
        {
            GetKeyboards().ForEach(keyboard =>
            {
                keyboard.onTextInput -= HandleTextInput;
                keyboard.onTextInput += HandleTextInput;
            });
        }

        private void DisableKeyboardTextInput()
        {
            GetKeyboards().ForEach(keyboard => keyboard.onTextInput -= HandleTextInput);
        }

        private List<Keyboard> GetKeyboards()
        {
            List<Keyboard> keyboards = new();
            foreach (InputDevice inputDevice in playerInput.devices)
            {
                if (inputDevice is Keyboard keyboard)
                {
                    keyboards.Add(keyboard);
                }
            }

            return keyboards;
        }

        private void HandleTextInput(char c) => OnKeyboardTextInput?.Invoke(c);

        private void HandleControlsChanged(PlayerInput pi)
        {
            ControlScheme? controlSchemeNullable = ControlSchemeNameToEnum(pi.currentControlScheme);
            if (controlSchemeNullable == null)
            {
                return;
            }

            ControlScheme controlScheme = controlSchemeNullable.Value;
            if (controlScheme == CurrentControlScheme)
            {
                return;
            }
            
            CurrentControlScheme = controlScheme;
            OnControlSchemeChanged?.Invoke(controlScheme);
        }
        
        private static ControlScheme? ControlSchemeNameToEnum(string controlSchemeName)
        {
            return controlSchemeName switch
            {
                // MARKER.ControlSchemeSwitch.Start
                "MouseKeyboard" => ControlScheme.MouseKeyboard,
                "Gamepad" => ControlScheme.Gamepad,
                // MARKER.ControlSchemeSwitch.End
                _ => null
            };
        }

        private void EnableMapsForContext(InputContext context)
        {
            switch (context)
            {
                // MARKER.EnableContextSwitchMembers.Start
                case InputContext.Gameplay:
                    DisableKeyboardTextInput();
                    gameplayMap.Enable();
                    gameplayMap.AddCallbacks(Gameplay);
                    pauseMenuMap.Disable();
                    pauseMenuMap.RemoveCallbacks(PauseMenu);
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                case InputContext.PauseMenu:
                    DisableKeyboardTextInput();
                    gameplayMap.Disable();
                    gameplayMap.RemoveCallbacks(Gameplay);
                    pauseMenuMap.Enable();
                    pauseMenuMap.AddCallbacks(PauseMenu);
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                case InputContext.AllInputDisabled:
                    DisableKeyboardTextInput();
                    gameplayMap.Disable();
                    gameplayMap.RemoveCallbacks(Gameplay);
                    pauseMenuMap.Disable();
                    pauseMenuMap.RemoveCallbacks(PauseMenu);
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                // MARKER.EnableContextSwitchMembers.End
                default:
                    throw new ArgumentOutOfRangeException(nameof(context), context, null);
            }
        }
        
        private void SetUIEventSystemActions(
            InputAction point,
            InputAction leftClick,
            InputAction middleClick,
            InputAction rightClick,
            InputAction scrollWheel,
            InputAction move,
            InputAction submit,
            InputAction cancel,
            InputAction trackedDevicePosition,
            InputAction trackedDeviceOrientation)
        {
            if (uiInputModule == null || !Input.UseContextEventSystemActions)
            {
                return;
            }
            
            uiInputModule.point = InputActionReference.Create(point);
            uiInputModule.leftClick = InputActionReference.Create(leftClick);
            uiInputModule.middleClick = InputActionReference.Create(middleClick);
            uiInputModule.rightClick = InputActionReference.Create(rightClick);
            uiInputModule.scrollWheel = InputActionReference.Create(scrollWheel);
            uiInputModule.move = InputActionReference.Create(move);
            uiInputModule.submit = InputActionReference.Create(submit);
            uiInputModule.cancel = InputActionReference.Create(cancel);
            uiInputModule.trackedDevicePosition = InputActionReference.Create(trackedDevicePosition);
            uiInputModule.trackedDeviceOrientation = InputActionReference.Create(trackedDeviceOrientation);
        }
        
        #endregion
    }
}
