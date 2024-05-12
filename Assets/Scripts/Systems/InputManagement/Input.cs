using System;
using System.Linq;
using StellarMass.Systems.InputManagement.Data;
using StellarMass.Systems.InputManagement.Generated.MapActions;
using StellarMass.Systems.InputManagement.Generated.MapCaches;
using StellarMass.Utilities;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
using UnityEditor;
#endif

//// NP TODO: Support multiple players, as an option in OfflineInputData
namespace StellarMass.Systems.InputManagement
{
    /// <summary>
    /// Main point of usage for all input in the game.
    /// DO NOT CHANGE the "MARKER" lines - they assist with code auto-generation.
    /// </summary>
    public static class Input
    {
        #region Fields & Properties
        // MARKER.RuntimeInputAddress.Start
        private const string RUNTIME_INPUT_DATA_ADDRESS = "RuntimeInputData";
        // MARKER.RuntimeInputAddress.End
        public static event Action<char> OnKeyboardTextInput;

        // The following 3 events will fire regardless of maps or keyboard input being enabled/disabled.
        public static event Action OnAnyButtonPressed;
        public static event Action<ControlScheme> OnControlSchemeChanged;
        public static event Action<InputDevice> OnLastUsedDeviceChanged;

        // MARKER.MapActionsProperties.Start
        public static GameplayActions Gameplay { get; private set; }
        public static PauseMenuActions PauseMenu { get; private set; }
        // MARKER.MapActionsProperties.End
        
        // MARKER.MapCacheFields.Start
        private static GameplayMapCache gameplayMap;
        private static PauseMenuMapCache pauseMenuMap;
        // MARKER.MapCacheFields.End

        private static RuntimeInputData runtimeInputData;
        private static InputSystemUIInputModule uiInputModule;
        private static IDisposable anyButtonPressListener;

        public static InputContext CurrentContext { get; private set; }
        public static ControlScheme CurrentControlScheme { get; private set; }
        public static InputDevice LastUsedDevice { get; private set; }
        
        // MARKER.DefaultContextProperty.Start
        private static InputContext DefaultContext => InputContext.Gameplay;
        // MARKER.DefaultContextProperty.End
        #endregion

        #region Setup
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= handlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += handlePlayModeStateChanged;

            void handlePlayModeStateChanged(PlayModeStateChange playModeStateChange)
            {
                if (playModeStateChange is PlayModeStateChange.ExitingPlayMode) Terminate();
            }
#else
            Application.quitting -= Terminate;
            Application.quitting += Terminate;
#endif
            runtimeInputData = AddressablesUtility.LoadAssetSynchronous<RuntimeInputData>(RUNTIME_INPUT_DATA_ADDRESS);
            InputActionAsset asset = runtimeInputData.InputActionAsset;
            if (asset == null)
            {
                throw new MissingFieldException($"Input manager's {nameof(runtimeInputData)} is missing an input action asset!");
            }

            // MARKER.MapAndActionsInstantiation.Start
            Gameplay = new GameplayActions();
            gameplayMap = new GameplayMapCache(asset);
            PauseMenu = new PauseMenuActions();
            pauseMenuMap = new PauseMenuMapCache(asset);
            // MARKER.MapAndActionsInstantiation.End
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            PlayerInput playerInput = Object.FindObjectOfType<PlayerInput>();
            uiInputModule = Object.FindObjectOfType<InputSystemUIInputModule>();

            if (playerInput == null || uiInputModule == null)
            {
                GameObject inputMgmtGameObject = new GameObject("InputManagement");
                if (playerInput == null)
                    playerInput = inputMgmtGameObject.AddComponent<PlayerInput>();
                if (uiInputModule == null)
                    uiInputModule = inputMgmtGameObject.AddComponent<InputSystemUIInputModule>();
                Object.DontDestroyOnLoad(inputMgmtGameObject);
            }
            
            playerInput.actions = runtimeInputData.InputActionAsset;
            playerInput.uiInputModule = uiInputModule;
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

            EnableContext(DefaultContext);
            AddSubscriptions();
        }
        
        private static void Terminate()
        {
            RemoveSubscriptions();
        }

        private static void AddSubscriptions()
        {
            InputSystem.onEvent += HandleEvent;
            InputSystem.onDeviceChange += HandleDeviceChange;
            InputUser.onChange += HandleInputUserChange;
            anyButtonPressListener = InputSystem.onAnyButtonPress.Call(HandleAnyButtonPressed);
        }
        
        private static void RemoveSubscriptions()
        {
            InputSystem.onEvent -= HandleEvent;
            InputSystem.onDeviceChange -= HandleDeviceChange;
            InputUser.onChange -= HandleInputUserChange;
            anyButtonPressListener.Dispose();
            
            RemoveAllMapActionCallbacks();
            DisableKeyboardTextInput();
            runtimeInputData.InputActionAsset.Disable();
        }

        private static void RemoveAllMapActionCallbacks()
        {
            // MARKER.MapActionsRemoveCallbacks.Start
            gameplayMap.RemoveCallbacks(Gameplay);
            pauseMenuMap.RemoveCallbacks(PauseMenu);
            // MARKER.MapActionsRemoveCallbacks.End
        }
        #endregion

        #region Public Interface

        public static void EnableContext(InputContext context)
        {
            CurrentContext = context;
            EnableMapsForContext(context);
        }
        
        public static bool TryGetBindingPathInfo(InputControl inputControl, out BindingPathInfo bindingPathInfo)
        {
            bindingPathInfo = default;

            ParseInputControlPath(inputControl, out string deviceName, out string controlPath);

            if (!runtimeInputData.BindingDataReferences.TryGetValue(deviceName, out BindingDataAssetReference bindingDataAssetReference))
            {
                return false;
            }

            BindingData bindingData = bindingDataAssetReference.LoadAssetSynchronous();
            if (bindingData == null)
            {
                return false;
            }

            if (!bindingData.BindingDisplayInfo.TryGetValue(controlPath, out bindingPathInfo))
            {
                return false;
            }

            if (!bindingPathInfo.OverrideDisplayName)
            {
                bindingPathInfo.DisplayName = inputControl.displayName;
            }

            return true;
        }

        public static void ChangeSubscription(InputActionReference actionReference, Action<InputAction.CallbackContext> callback, bool subscribe)
        {
            if (actionReference == null)
            {
                Debug.LogError("Trying to subscribe to a nonexistent action reference.");
                return;
            }

            if (callback == null)
            {
                Debug.LogError("Trying to subscribe with a nonexistent callback.");
                return;
            }
            
            FindActionEventAndSubscribe(actionReference, callback, subscribe);
        }
        #endregion

        #region Private Runtime Functionality
        private static void EnableMapsForContext(InputContext context)
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
        
        private static void EnableKeyboardTextInput()
        {
            Keyboard.current.onTextInput -= HandleTextInput;
            Keyboard.current.onTextInput += HandleTextInput;
        }

        private static void DisableKeyboardTextInput() => Keyboard.current.onTextInput -= HandleTextInput;

        private static void SetUIEventSystemActions(
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
            if (runtimeInputData == null || !runtimeInputData.UseContextEventSystemActions)
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

        private static void ParseInputControlPath(InputControl inputControl, out string deviceName, out string controlPath)
        {
            deviceName = inputControl.device.name;
            controlPath = inputControl.path[(2 + deviceName.Length)..];
        }
        
        private static void HandleAnyButtonPressed(InputControl inputControl) => OnAnyButtonPressed?.Invoke();
        private static void HandleTextInput(char c) => OnKeyboardTextInput?.Invoke(c);
        
        private static void HandleDeviceChange(InputDevice device, InputDeviceChange inputDeviceChange)
        {
            if (LastUsedDevice == device)
            {
                return;
            }

            LastUsedDevice = device;
            OnLastUsedDeviceChanged?.Invoke(device);
        }

        private static void HandleEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (LastUsedDevice == device)
            {
                return;
            }

            // Prevents triggering events for some devices which spit out noise, such as PS4/PS5 gamepads with gyroscopes
            if (device.noisy && eventPtr.type == StateEvent.Type &&
                !eventPtr.EnumerateChangedControls(device: device, magnitudeThreshold: 0.0001f).Any())
            {
                return;
            }

            LastUsedDevice = device;
            OnLastUsedDeviceChanged?.Invoke(device);
        }
        
        /// This will only be called if PlayerInput exists.
        private static void HandleInputUserChange(InputUser user, InputUserChange change, InputDevice device)
        {
            if (change is not InputUserChange.ControlSchemeChanged || user.controlScheme == null)
            {
                return;
            }

            ControlScheme? controlScheme = ControlSchemeNameToEnum(user.controlScheme.Value.name);
            if (controlScheme == null)
            {
                return;
            }

            CurrentControlScheme = controlScheme.Value;
            OnControlSchemeChanged?.Invoke(controlScheme.Value);
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

        private static void FindActionEventAndSubscribe(InputActionReference actionReference, Action<InputAction.CallbackContext> callback, bool subscribe)
        {
            InputAction action = actionReference.action;
            InputActionMap map = action.actionMap;

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
    }
}
