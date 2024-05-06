// MARKER.UsingDirective.Start
using StellarMass.InputManagement.UnityGenerated;
// MARKER.UsingDirective.End
using System;
using System.Linq;
using StellarMass.InputManagement.Data;
using StellarMass.InputManagement.Maps;
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

//// NP TODO: In order of priority:
//// - Full event system swapping support, w/ runtime data checkbox option. May require using on-disk asset only, because UI input module only takes InputActionReference.
namespace StellarMass.InputManagement
{
    public static class InputManager
    {
        #region Fields & Properties
        // MARKER.RuntimeInputDataAddress.Start
        private const string RUNTIME_INPUT_DATA_ADDRESS = "RuntimeInputData";
        // MARKER.RuntimeInputDataAddress.End

        // These events are the only ones that can fire independently of specific action maps being enabled or disabled.
        public static event Action OnAnyButtonPressed;
        public static event Action<char> OnKeyboardTextInput;
        public static event Action<ControlScheme> OnControlSchemeChanged;
        public static event Action<InputDevice> OnLastUsedDeviceChanged;

        // MARKER.MapActionsProperties.Start
        public static GameplayActions Gameplay { get; private set; }
        public static PauseMenuActions PauseMenu { get; private set; }
        // MARKER.MapActionsProperties.End
        
        // MARKER.InputActionCollectionDeclaration.Start
        private static InputActions inputActions;
        // MARKER.InputActionCollectionDeclaration.End
        
        private static RuntimeInputData runtimeInputData;
        private static InputSystemUIInputModule uiInputModule;
        private static IDisposable anyButtonPressListener;

        private static InputContext previousContext;
        public static InputContext CurrentContext { get; private set; }
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
            
            // MARKER.InputActionCollectionInstantiation.Start
            inputActions = new InputActions();
            // MARKER.InputActionCollectionInstantiation.End

            // MARKER.InstantiateMapInstances.Start
            Gameplay = new GameplayActions();
            PauseMenu = new PauseMenuActions();
            // MARKER.InstantiateMapInstances.End
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
            
            playerInput.actions = inputActions.asset;
            playerInput.uiInputModule = uiInputModule;
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

            previousContext = DefaultContext;
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
            inputActions.Disable();
        }

        private static void RemoveAllMapActionCallbacks()
        {
            // MARKER.MapActionsRemoveCallbacks.Start
            inputActions.Gameplay.RemoveCallbacks(Gameplay);
            inputActions.PauseMenu.RemoveCallbacks(PauseMenu);
            // MARKER.MapActionsRemoveCallbacks.End
        }
        
        #endregion

        #region Public Interface
        
        public static void EnableKeyboardTextInput() => Keyboard.current.onTextInput += HandleTextInput;
        public static void DisableKeyboardTextInput() => Keyboard.current.onTextInput -= HandleTextInput;

        public static void EnablePreviousContext() => EnableContext(previousContext);
        
        public static void EnableContext(InputContext context)
        {
            previousContext = CurrentContext;
            CurrentContext = context;

            switch (context)
            {
                // MARKER.EnableContextSwitchMembers.Start
                case InputContext.Gameplay:
                    inputActions.Gameplay.Enable();
                    inputActions.Gameplay.AddCallbacks(Gameplay);
                    inputActions.PauseMenu.Disable();
                    inputActions.PauseMenu.RemoveCallbacks(PauseMenu);
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                case InputContext.PauseMenu:
                    inputActions.Gameplay.Disable();
                    inputActions.Gameplay.RemoveCallbacks(Gameplay);
                    inputActions.PauseMenu.Enable();
                    inputActions.PauseMenu.AddCallbacks(PauseMenu);
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                case InputContext.AllInputDisabled:
                    inputActions.Gameplay.Disable();
                    inputActions.Gameplay.RemoveCallbacks(Gameplay);
                    inputActions.PauseMenu.Disable();
                    inputActions.PauseMenu.RemoveCallbacks(PauseMenu);
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                // MARKER.EnableContextSwitchMembers.End
                default:
                    throw new ArgumentOutOfRangeException(nameof(context), context, null);
            }
        }
                
        public static InputAction GetLocalAssetActionFromReference(ActionReferenceWrapper referenceWrapper)
        {
            return inputActions.asset.FindActionMap(referenceWrapper.MapName).FindAction(referenceWrapper.ActionName);
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

            return true;
        }

        #endregion

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

            ControlScheme? enumValue = ControlSchemeNameToEnum(user.controlScheme.Value.name);
            if (enumValue == null)
            {
                return;
            }
            
            OnControlSchemeChanged?.Invoke(enumValue.Value);
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

        private static void SetUIEventSystemActions(
            InputActionReference point,
            InputActionReference leftClick,
            InputActionReference middleClick,
            InputActionReference rightClick,
            InputActionReference scrollWheel,
            InputActionReference move,
            InputActionReference submit,
            InputActionReference cancel,
            InputActionReference trackedDevicePosition,
            InputActionReference trackedDeviceOrientation)
        {
            if (runtimeInputData != null && runtimeInputData.UseContextEventSystemActions)
            {
                return;
            }
            
            uiInputModule.point = point;
            uiInputModule.leftClick = leftClick;
            uiInputModule.middleClick = middleClick;
            uiInputModule.rightClick = rightClick;
            uiInputModule.scrollWheel = scrollWheel;
            uiInputModule.move = move;
            uiInputModule.submit = submit;
            uiInputModule.cancel = cancel;
            uiInputModule.trackedDevicePosition = trackedDevicePosition;
            uiInputModule.trackedDeviceOrientation = trackedDeviceOrientation;
        }
    }
}
