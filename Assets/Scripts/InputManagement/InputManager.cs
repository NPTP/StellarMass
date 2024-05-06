using System;
using System.Collections.Generic;
using StellarMass.InputManagement.Data;
using StellarMass.InputManagement.MapInstances;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;
// MARKER.UsingDirective.Start
using StellarMass.InputManagement.UnityGenerated;
// MARKER.UsingDirective.End

#if UNITY_EDITOR
using UnityEditor;
#endif

//// NP TODO: In order of priority:
//// - A way to use input action assets in the project and have them run through here so they use the correct asset
//// - Find currently used device and send event when it changes. Don't require pressing mapped buttons to do so.
//// - Runtime data loaded by addressable.
//// - Define icons & text (with localized strings) for each binding. Uses a serialized dictionary. In runtime data.
//// - Full event system swapping support, w/ runtime data checkbox option. May require using on-disk asset only, because UI input module only takes InputActionReference.
//// - Fill out null entries in ControlTypeTranslator with correct types
namespace StellarMass.InputManagement
{
    public static class InputManager
    {
        // MARKER.RuntimeInputDataPath.Start
        private const string RUNTIME_INPUT_DATA_PATH = "Assets/ScriptableObjects/RuntimeData/RuntimeInputData.asset";
        // MARKER.RuntimeInputDataPath.End

        public static event Action OnAnyButtonPressed;
        public static event Action<char> OnKeyboardTextInput;
        public static event Action<ControlScheme> OnControlSchemeChanged;

        // MARKER.MapInstanceProperties.Start
        public static Gameplay Gameplay { get; private set; }
        public static PauseMenu PauseMenu { get; private set; }
        // MARKER.MapInstanceProperties.End

        // MARKER.InputActionCollectionDeclaration.Start
        private static InputActions inputActions;
        // MARKER.InputActionCollectionDeclaration.End

        private static RuntimeInputData runtimeInputData;
        private static InputSystemUIInputModule uiInputModule;
        private static List<MapInstance> mapInstances;
        private static IDisposable anyButtonPressListener;
        private static InputDevice lastUsedDevice;

        private static InputContext previousContext;
        public static InputContext CurrentContext { get; private set; }
        
        // MARKER.DefaultContextProperty.Start
        private static InputContext DefaultContext => InputContext.Gameplay;
        // MARKER.DefaultContextProperty.End

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
            // NP TODO: Load Runtime data w/addressables
            // runtimeInputData = Addressables.Load...etc RUNTIME_INPUT_DATA_PATH;

            // MARKER.InputActionCollectionInstantiation.Start
            inputActions = new InputActions();
            // MARKER.InputActionCollectionInstantiation.End

            // MARKER.InstantiateMapInstances.Start
            Gameplay = new Gameplay(inputActions.Gameplay);
            PauseMenu = new PauseMenu(inputActions.PauseMenu);
            // MARKER.InstantiateMapInstances.End

            mapInstances = new List<MapInstance>
            {
                // MARKER.CollectionInitializer.Start
                Gameplay,
                PauseMenu,
                // MARKER.CollectionInitializer.End
            };
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            GameObject inputMgmtGameObject = new GameObject("InputManagement");
            PlayerInput playerInput = inputMgmtGameObject.AddComponent<PlayerInput>();
            uiInputModule = inputMgmtGameObject.AddComponent<InputSystemUIInputModule>();

            Object.DontDestroyOnLoad(inputMgmtGameObject);
            
            playerInput.actions = inputActions.asset;
            playerInput.uiInputModule = uiInputModule;
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

            previousContext = DefaultContext;
            EnableContext(DefaultContext);
            AddSubscriptions();
        }

        private static void AddSubscriptions()
        {
            InputUser.onChange += HandleInputUserChange;
            anyButtonPressListener = InputSystem.onAnyButtonPress.Call(HandleAnyButtonPressed);
        }

        private static void RemoveSubscriptions()
        {
            mapInstances.ForEach(m => m.Terminate());

            InputUser.onChange += HandleInputUserChange;
            anyButtonPressListener.Dispose();
            DisableKeyboardTextInput();

            inputActions.Disable();
        }

        private static void Terminate()
        {
            RemoveSubscriptions();
        }

        private static void HandleAnyButtonPressed(InputControl inputControl) => OnAnyButtonPressed?.Invoke();
        public static void EnableKeyboardTextInput() => Keyboard.current.onTextInput += HandleTextInput;
        public static void DisableKeyboardTextInput() => Keyboard.current.onTextInput -= HandleTextInput;
        private static void HandleTextInput(char c) => OnKeyboardTextInput?.Invoke(c);

        public static void AddListenerFromReference(InputActionReference reference, Action<InputAction.CallbackContext> handler)
        {
            InputAction inputAction = GetLocalAssetActionFromReference(reference); 
            inputAction.started += handler;
            inputAction.performed += handler;
            inputAction.canceled += handler;
        }
        
        public static void RemoveListenerFromReference(InputActionReference reference, Action<InputAction.CallbackContext> handler)
        {
            InputAction inputAction = GetLocalAssetActionFromReference(reference); 
            inputAction.started -= handler;
            inputAction.performed -= handler;
            inputAction.canceled -= handler;
        }
        
        private static InputAction GetLocalAssetActionFromReference(InputActionReference reference)
        {
            string map = reference.action.actionMap.name;
            string action = reference.action.name;
            return inputActions.asset.FindActionMap(map).FindAction(action);
        }

        /// <summary>
        /// This will only be called if PlayerInput is being used.
        /// </summary>
        private static void HandleInputUserChange(InputUser user, InputUserChange change, InputDevice device)
        {
            if (change is InputUserChange.ControlSchemeChanged && user.controlScheme != null)
            {
                ControlScheme? enumValue = ControlSchemeNameToEnum(user.controlScheme.Value.name);
                if (enumValue != null)
                {
                    OnControlSchemeChanged?.Invoke(enumValue.Value);
                    Debug.Log($"Just changed to: {enumValue.Value}");
                }
            }
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

        public static void EnableContext(InputContext context)
        {
            previousContext = CurrentContext;
            CurrentContext = context;

            switch (context)
            {
                // MARKER.EnableContextSwitchMembers.Start
                case InputContext.Gameplay:
                    Gameplay.Enable();
                    PauseMenu.Disable();
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                case InputContext.PauseMenu:
                    Gameplay.Disable();
                    PauseMenu.Enable();
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                case InputContext.AllInputDisabled:
                    Gameplay.Disable();
                    PauseMenu.Disable();
                    SetUIEventSystemActions(null, null, null, null, null, null, null, null, null, null);
                    break;
                // MARKER.EnableContextSwitchMembers.End
                default:
                    throw new ArgumentOutOfRangeException(nameof(context), context, null);
            }
        }

        public static void EnablePreviousContext()
        {
            EnableContext(previousContext);
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
