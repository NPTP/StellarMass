using System;
using System.Collections.Generic;
using NPTP.InputSystemWrapper.Actions;
using NPTP.InputSystemWrapper.Enums;
using NPTP.InputSystemWrapper.Generated.Actions;
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

        /// <summary>
        /// Corresponds to InputUser.onChange, for this player specifically.
        /// </summary>
        public event Action<InputUserChangeInfo> OnInputUserChange;
        
        public event Action<ControlScheme> OnControlSchemeChanged;

        /// <summary>
        /// The input player can be used when enabled, and is ignored when disabled.
        /// </summary>
        public event Action<InputPlayer> OnEnabledOrDisabled;
        
        /// <summary>
        /// Sends the keyboard text character that was just input by this player,
        /// but only if the current InputContext that allows keyboard text input is active.
        /// </summary>
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
                    InputContext = inputContext;
                else
                    Asset.Disable();
                // UpdateLastUsedDevice();
                OnEnabledOrDisabled?.Invoke(this);
            }
        }
        
        private InputContext inputContext;
        public InputContext InputContext
        {
            get => inputContext;
            set
            {
                inputContext = value;
                EnableMapsForContext(value);
            }
        }

        public PlayerID ID { get; }
        public ControlScheme CurrentControlScheme { get; private set; }
        
        // MARKER.ActionsProperties.Start
        public GameplayActions Gameplay { get; }
        public MenuActions Menu { get; }
        // MARKER.ActionsProperties.End

        private InputDevice lastUsedDevice;
        internal InputDevice LastUsedDevice
        {
            get
            {
                UpdateLastUsedDevice();
                return lastUsedDevice;
            }
        }
        
        internal InputActionAsset Asset { get; }
        
        private ReadOnlyArray<InputDevice> PairedDevices => playerInput == null ? new ReadOnlyArray<InputDevice>() : playerInput.devices;
        
        private GameObject playerInputGameObject;
        private PlayerInput playerInput;
        private InputSystemUIInputModule uiInputModule;
        
        #endregion
        
        #region Setup & Teardown

        internal InputPlayer(InputActionAsset asset, PlayerID id, bool isMultiplayer, Transform parent)
        {
            Asset = InstantiateNewActions(asset);
            ID = id;
            
            // MARKER.ActionsInstantiation.Start
            Gameplay = new GameplayActions(Asset);
            Menu = new MenuActions(Asset);
            // MARKER.ActionsInstantiation.End
            
            SetUpInputPlayerGameObject(isMultiplayer, parent);
            
            SetEventSystemActions();
        }
        
        internal void Terminate()
        {
            Asset.Disable();
            DisableKeyboardTextInput();
            DisableAllMapsAndRemoveCallbacks();
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
            uiInputModule.actionsAsset = Asset;
            
            playerInput.actions = Asset;
            playerInput.uiInputModule = uiInputModule;
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                        // Set this manually because the initial control scheme gets set before we are able to respond to it with event handlers.

            CurrentControlScheme = playerInput.currentControlScheme.ToControlSchemeEnum();
        }

        private void SetEventSystemActions()
        {
            // MARKER.EventSystemActions.Start
            uiInputModule.move = createLocalAssetReference("33d7d08d-c733-4d29-8a19-3414d8312c5f");
            uiInputModule.submit = createLocalAssetReference("9d40871e-5615-4343-a832-dc37fd431297");
            uiInputModule.cancel = createLocalAssetReference("7593878a-c79e-4e90-b212-30bf9ac46ddb");
            // MARKER.EventSystemActions.End

#pragma warning disable CS8321
            InputActionReference createLocalAssetReference(string actionID)
#pragma warning restore CS8321
            {
                return string.IsNullOrEmpty(actionID)
                    ? null
                    : InputActionReference.Create(Asset.FindAction(actionID, throwIfNotFound: false));
            }
        }

        #endregion

        #region Internal
        
        internal bool IsDevicePaired(InputDevice device)
        {
            return PairedDevices.ContainsReference(device);
        }
        
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
            // UpdateLastUsedDevice();
        }

        internal void UnpairDevice(InputDevice device)
        {
            if (playerInput == null || !playerInput.user.valid)
            {
                return;
            }
            
            playerInput.user.UnpairDevice(device);
            // UpdateLastUsedDevice();
        }

        internal void UnpairDevices()
        {
            if (playerInput == null || !playerInput.user.valid)
            {
                return;
            }
            
            playerInput.user.UnpairDevices();
            // UpdateLastUsedDevice();
        }

        internal void EnableAutoSwitching(bool enable)
        {
            if (playerInput == null)
            {
                return;
            }

            playerInput.neverAutoSwitchControlSchemes = !enable;
        }
        
        /// <summary>
        /// Called by the InputPlayerCollection. If we got here, it means we have already checked that the input user
        /// experiencing a change refers to this player.
        /// </summary>
        internal void HandleInputUserChange(InputUserChange inputUserChange, InputDevice inputDevice)
        {
            if (playerInput == null)
            {
                return;
            }

            UpdateLastUsedDevice(inputDevice);
            ControlScheme previousControlScheme = CurrentControlScheme;
            CurrentControlScheme = playerInput.currentControlScheme.ToControlSchemeEnum();
            if (previousControlScheme != CurrentControlScheme)
            {
                OnControlSchemeChanged?.Invoke(CurrentControlScheme);
            }
            
            OnInputUserChange?.Invoke(new InputUserChangeInfo(this, inputUserChange));
        }

        internal bool TryGetMapAndActionInPlayerAsset(InputAction actionFromReference, out InputActionMap map, out InputAction action)
        {
            action = null;
            map = null;

            if (actionFromReference == null)
                return false;

            map = Asset.FindActionMap(actionFromReference.actionMap.name);
            if (map == null)
                return false;
            
            action = map.FindAction(actionFromReference.name);
            return action != null;
        }
        
        internal ActionWrapper FindActionWrapper(ActionReference actionReference)
        {
            if (!TryGetMapAndActionInPlayerAsset(actionReference.InternalAction, out InputActionMap map, out InputAction action))
            {
                return null;
            }
            
            // The auto-generated code below ensures that the action used is from the correct asset AND behaves
            // identically to all direct action subscriptions in this wrapper system (where double subs are
            // prevented, subs to actions can be made at any time regardless of map/action/player state, and the
            // event signatures look the same as the ones subscribed to manually).

            // MARKER.FindActionWrapperIfElse.Start
            if (Gameplay.ActionMap == map)
            {
                if (action == Gameplay.Thrust.InputAction) return Gameplay.Thrust;
                if (action == Gameplay.Shoot.InputAction) return Gameplay.Shoot;
                if (action == Gameplay.Hyperspace.InputAction) return Gameplay.Hyperspace;
                if (action == Gameplay.Turn.InputAction) return Gameplay.Turn;
                if (action == Gameplay.Pause.InputAction) return Gameplay.Pause;
            }
            else if (Menu.ActionMap == map)
            {
                if (action == Menu.Navigate.InputAction) return Menu.Navigate;
                if (action == Menu.Submit.InputAction) return Menu.Submit;
                if (action == Menu.Cancel.InputAction) return Menu.Cancel;
                if (action == Menu.Unpause.InputAction) return Menu.Unpause;
            }
            // MARKER.FindActionWrapperIfElse.End

            return null;
        }

        #endregion

        #region Private
        
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
        
        // TODO (optimization): Currently commented out in this class in a few places, since enabling/disabling PlayerInput,
        // pairing/unpairing devices, etc. should all call HandleInputUserChange. Uncomment those calls if HandleInputUserChange
        // isn't cutting it, and delete the commented calls outright if it is!
        private void UpdateLastUsedDevice(InputDevice fallbackDevice = null)
        {
            ReadOnlyArray<InputDevice> pairedDevices = PairedDevices;
            
            if (pairedDevices.Count == 0)
            {
                lastUsedDevice = null;
            }
            else if (pairedDevices.Count == 1 ||
                     (pairedDevices.Count > 1 && (lastUsedDevice == null || !pairedDevices.ContainsReference(lastUsedDevice))))
            {
                lastUsedDevice = pairedDevices[0];
            }
            else if (fallbackDevice != null)
            {
                lastUsedDevice = fallbackDevice;
            }
        }
        
        private void DisableAllMapsAndRemoveCallbacks()
        {
            // MARKER.DisableAllMapsAndRemoveCallbacksBody.Start
            Gameplay.DisableAndUnregisterCallbacks();
            Menu.DisableAndUnregisterCallbacks();
            // MARKER.DisableAllMapsAndRemoveCallbacksBody.End
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
                    Gameplay.DisableAndUnregisterCallbacks();
                    Menu.DisableAndUnregisterCallbacks();
                    break;
                case InputContext.Gameplay:
                    DisableKeyboardTextInput();
                    Gameplay.EnableAndRegisterCallbacks();
                    Menu.DisableAndUnregisterCallbacks();
                    break;
                case InputContext.Menu:
                    DisableKeyboardTextInput();
                    Gameplay.EnableAndRegisterCallbacks();
                    Menu.DisableAndUnregisterCallbacks();
                    break;
                // MARKER.EnableContextSwitchMembers.End
                default:
                    throw new ArgumentOutOfRangeException(nameof(context), context, null);
            }
        }

        #endregion
    }
}
