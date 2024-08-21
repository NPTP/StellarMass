using System;
using System.Collections.Generic;
using NPTP.InputSystemWrapper.Enums;
using NPTP.InputSystemWrapper.Generated.MapActions;
using NPTP.InputSystemWrapper.Generated.MapCaches;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;

namespace NPTP.InputSystemWrapper
{
    public sealed class InputPlayer
    {
        #region Field & Properties

        public event Action<DeviceControlInfo> OnDeviceControlChanged;
        public event Action<InputPlayer> OnEnabledOrDisabled;
        public event Action<char> OnKeyboardTextInput;

        private bool enabled;
        public bool Enabled
        {
            get => enabled;
            internal set
            {
                if (playerInput == null)
                {
                    return;
                }
            
                enabled = value;
                playerInputGameObject.SetActive(value);
                if (value)
                    CurrentContext = currentContext;
                else
                    asset.Disable();
                UpdateLastUsedDevice();
                OnEnabledOrDisabled?.Invoke(this);
            }
        }

        public PlayerID ID { get; }
        private InputContext currentContext;
        public InputContext CurrentContext
        {
            get => currentContext;
            set
            {
                currentContext = value;
                EnableMapsForContext(value);
            }
        }

        // TODO: Still relevant? Can we get rid of it?
        public ControlScheme CurrentControlScheme { get; private set; }

        public InputDevice LastUsedDevice { get; private set; }

        private ReadOnlyArray<InputDevice> PairedDevices => playerInput == null ? new ReadOnlyArray<InputDevice>() : playerInput.devices;
        public bool IsDevicePaired(InputDevice device) => PairedDevices.ContainsReference(device);

        // MARKER.MapActionsProperties.Start
        public GameplayActions Gameplay { get; }
        public PauseMenuActions PauseMenu { get; }
        // MARKER.MapActionsProperties.End
        
        // MARKER.MapCacheFields.Start
        private readonly GameplayMapCache gameplayMap;
        private readonly PauseMenuMapCache pauseMenuMap;
        // MARKER.MapCacheFields.End

        private readonly InputActionAsset asset;
        private GameObject playerInputGameObject;
        private PlayerInput playerInput;
        private InputSystemUIInputModule uiInputModule;
        private ControlScheme previousControlScheme;

        #endregion
        
        #region Setup & Teardown

        internal InputPlayer(InputActionAsset inputActionAsset, PlayerID id, bool isMultiplayer, Transform parent)
        {
            asset = InstantiateNewActions(inputActionAsset);
            ID = id;

            // MARKER.MapAndActionsInstantiation.Start
            Gameplay = new GameplayActions();
            gameplayMap = new GameplayMapCache(asset);
            PauseMenu = new PauseMenuActions();
            pauseMenuMap = new PauseMenuMapCache(asset);
            // MARKER.MapAndActionsInstantiation.End

            SetUpInputPlayerGameObject(isMultiplayer, parent);
            
            SetEventSystemActions();

            // TODO: We are keeping track of the last used device with other methods now, can we get rid of this?
            playerInput.onActionTriggered += HandleAnyActionTriggered;
        }
        
        internal void Terminate()
        {
            playerInput.onActionTriggered -= HandleAnyActionTriggered;
            asset.Disable();
            DisableKeyboardTextInput();
            RemoveAllMapActionCallbacks();
            Object.Destroy(playerInputGameObject);
        }

        private InputActionAsset InstantiateNewActions(InputActionAsset actions)
        {
            InputActionAsset oldActions = actions;
            InputActionAsset newActions = Object.Instantiate(actions);
            for (int actionMap = 0; actionMap < oldActions.actionMaps.Count; actionMap++)
            {
                for (int binding = 0; binding < oldActions.actionMaps[actionMap].bindings.Count; binding++)
                {
                    newActions.actionMaps[actionMap].ApplyBindingOverride(binding, oldActions.actionMaps[actionMap].bindings[binding]);
                }
            }

            return newActions;
        }
        
        private void SetUpInputPlayerGameObject(bool isMultiplayer, Transform parent)
        {
            if (playerInputGameObject != null)
            {
                return;
            }
            
            playerInputGameObject = new GameObject
            {
                name = $"{ID}Input",
                transform = { position = Vector3.zero, parent = parent}
            };

            playerInput = playerInputGameObject.AddComponent<PlayerInput>();
            playerInput.neverAutoSwitchControlSchemes = isMultiplayer;
                
            if (isMultiplayer)
                playerInputGameObject.AddComponent<MultiplayerEventSystem>();
            else
                playerInputGameObject.AddComponent<EventSystem>();
                
            uiInputModule = playerInputGameObject.AddComponent<InputSystemUIInputModule>();
            uiInputModule.actionsAsset = asset;
            
            playerInput.actions = asset;
            playerInput.uiInputModule = uiInputModule;
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            
            CurrentContext = currentContext;
        }

        private void SetEventSystemActions()
        {
            // MARKER.EventSystemActions.Start
            // MARKER.EventSystemActions.End

#pragma warning disable CS8321
            InputActionReference createLocalAssetReference(string actionID)
#pragma warning restore CS8321
            {
                return string.IsNullOrEmpty(actionID)
                    ? null
                    : InputActionReference.Create(asset.FindAction(actionID, throwIfNotFound: false));
            }
        }

        #endregion
        
        #region Internal Interface
        
        internal bool IsUser(InputUser user)
        {
            return playerInput != null && playerInput.user.id == user.id;
        }

        internal void PairDevice(InputDevice device)
        {
            if (playerInput == null || !playerInput.user.valid)
            {
                return;
            }
         
            InputUser.PerformPairingWithDevice(device, playerInput.user);
            UpdateLastUsedDevice();
        }

        internal void UnpairDevice(InputDevice device)
        {
            if (playerInput == null || !playerInput.user.valid)
            {
                return;
            }
            
            playerInput.user.UnpairDevice(device);
            UpdateLastUsedDevice();
        }

        internal void UnpairDevices()
        {
            if (playerInput == null || !playerInput.user.valid)
            {
                return;
            }
            
            playerInput.user.UnpairDevices();
            UpdateLastUsedDevice();
        }

        internal void EnableAutoSwitching(bool enable)
        {
            if (playerInput == null)
            {
                return;
            }

            playerInput.neverAutoSwitchControlSchemes = !enable;
        }
        
        internal void HandleInputUserChange(InputUserChange inputUserChange, InputDevice inputDevice)
        {
            if (playerInput == null)
            {
                return;
            }
            
            switch (inputUserChange)
            {
                case InputUserChange.ControlSchemeChanged:
                    // TODO: ControlScheme update is basically getting ignored in favour of checking paired devices below. Remove ControlScheme tracking?
                    CurrentControlScheme = ControlSchemeNameToEnum(playerInput.currentControlScheme);
                    goto deviceChange;
                case InputUserChange.DevicePaired:
                case InputUserChange.DeviceUnpaired:
                case InputUserChange.ControlsChanged: // When user bindings have changed (among other things)
                deviceChange:
                    InputDevice previousDevice = LastUsedDevice;
                    UpdateLastUsedDevice(inputDevice);
                    if (previousDevice == LastUsedDevice || (previousDevice is Mouse or Keyboard && LastUsedDevice is Mouse or Keyboard))
                        break;
                    OnDeviceControlChanged?.Invoke(new DeviceControlInfo(this, inputUserChange));
                    break;
            }
        }
        
        internal void FindActionEventAndSubscribe(InputActionReference actionReference, Action<InputAction.CallbackContext> callback, bool subscribe)
        {
            InputActionMap map = asset.FindActionMap(actionReference.action.actionMap.name);
            if (map == null) return;
            InputAction action = map.FindAction(actionReference.action.name);
            if (action == null) return;
            
            // The auto-generated code below ensures that the action used is from the correct asset AND behaves
            // identically to all direct action subscriptions in this wrapper system (where double subs are
            // prevented, subs to actions can be made at any time regardless of map/action/player state, and the
            // event signatures look the same as the ones subscribed to manually).

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
        
        private void HandleAnyActionTriggered(InputAction.CallbackContext context)
        {
            LastUsedDevice = context.control.device;
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
        
        // TODO: Check if this only needs to be used in one place (HandleInputUserChange) instead of 5 places, since enabling/disabling PlayerInput,
        // pairing/unpairing devices, these will all call HandleInputUserChange, right? Check, and avoid redundancy if so
        private void UpdateLastUsedDevice(InputDevice fallbackDevice = null)
        {
            ReadOnlyArray<InputDevice> pairedDevices = PairedDevices;
            if (pairedDevices.Count == 0)
            {
                LastUsedDevice = null;
            }
            else if (pairedDevices.Count == 1 ||
                     (pairedDevices.Count > 1 && (LastUsedDevice == null || !pairedDevices.ContainsReference(LastUsedDevice))))
            {
                LastUsedDevice = pairedDevices[0];
            }
            else if (fallbackDevice != null)
            {
                LastUsedDevice = fallbackDevice;
            }
        }
        
        private void RemoveAllMapActionCallbacks()
        {
            // MARKER.MapActionsRemoveCallbacks.Start
            gameplayMap.RemoveCallbacks(Gameplay);
            pauseMenuMap.RemoveCallbacks(PauseMenu);
            // MARKER.MapActionsRemoveCallbacks.End
        }
        
        private List<Keyboard> GetKeyboards()
        {
            List<Keyboard> keyboards = new();
            if (playerInput == null)
            {
                return keyboards;
            }
            
            foreach (InputDevice inputDevice in playerInput.devices)
            {
                if (inputDevice is Keyboard keyboard)
                {
                    keyboards.Add(keyboard);
                }
            }

            return keyboards;
        }

        private void HandleTextInput(char c)
        {
            OnKeyboardTextInput?.Invoke(c);
        }

        private static ControlScheme ControlSchemeNameToEnum(string controlSchemeName)
        {
#pragma warning disable CS8509
            return controlSchemeName switch
#pragma warning restore CS8509
            {
                // MARKER.ControlSchemeSwitch.Start
                "MouseKeyboard" => ControlScheme.MouseKeyboard,
                "Gamepad" => ControlScheme.Gamepad,
                // MARKER.ControlSchemeSwitch.End
            };
        }

        private void EnableMapsForContext(InputContext context)
        {
            if (!Enabled)
            {
                return;
            }
            
            switch (context)
            {
                // MARKER.EnableContextSwitchMembers.Start
                case InputContext.AllInputDisabled:
                    DisableKeyboardTextInput();
                    gameplayMap.Disable();
                    gameplayMap.RemoveCallbacks(Gameplay);
                    pauseMenuMap.Disable();
                    pauseMenuMap.RemoveCallbacks(PauseMenu);
                    break;
                case InputContext.Gameplay:
                    DisableKeyboardTextInput();
                    gameplayMap.Enable();
                    gameplayMap.AddCallbacks(Gameplay);
                    pauseMenuMap.Disable();
                    pauseMenuMap.RemoveCallbacks(PauseMenu);
                    break;
                case InputContext.UI:
                    DisableKeyboardTextInput();
                    gameplayMap.Disable();
                    gameplayMap.RemoveCallbacks(Gameplay);
                    pauseMenuMap.Enable();
                    pauseMenuMap.AddCallbacks(PauseMenu);
                    break;
                // MARKER.EnableContextSwitchMembers.End
                default:
                    throw new ArgumentOutOfRangeException(nameof(context), context, null);
            }
        }

        #endregion

        // TODO: Remove section after debugging finished
        #region Editor Debugging

#if UNITY_EDITOR
        public bool EDITOR_Enabled
        {
            set => Enabled = value;
        }
#endif

        #endregion
    }
}
