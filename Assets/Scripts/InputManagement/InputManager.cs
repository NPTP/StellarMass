using System;
using StellarMass.InputManagement.Data;
using StellarMass.InputManagement.Maps;
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
//// - Find currently used device and send event when it changes. Don't require pressing mapped buttons to do so.
//// - Runtime data loaded by addressable.
//// - Define icons & text (with localized strings) for each binding. Uses a serialized dictionary. In runtime data.
//// - Full event system swapping support, w/ runtime data checkbox option. May require using on-disk asset only, because UI input module only takes InputActionReference.
namespace StellarMass.InputManagement
{
    public static class InputManager
    {
        #region Fields & Properties
        // MARKER.RuntimeInputDataPath.Start
        private const string RUNTIME_INPUT_DATA_PATH = "Assets/ScriptableObjects/RuntimeData/RuntimeInputData.asset";
        // MARKER.RuntimeInputDataPath.End

        // These events are the only ones that can fire independently of specific action maps being enabled or disabled.
        public static event Action OnAnyButtonPressed;
        public static event Action<char> OnKeyboardTextInput;
        public static event Action<ControlScheme> OnControlSchemeChanged;

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
        private static InputDevice lastUsedDevice;

        private static InputContext previousContext;
        public static InputContext CurrentContext { get; private set; }
        
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
            // NP TODO: Load Runtime data w/addressables
            // runtimeInputData = Addressables.Load...etc RUNTIME_INPUT_DATA_PATH;

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
        
        private static void Terminate()
        {
            RemoveSubscriptions();
        }

        private static void RemoveSubscriptions()
        {
            // NP TODO: Cover this
            // MARKER.MapActionsRemoveCallbacks.Start
            // MARKER.MapActionsRemoveCallbacks.End

            InputUser.onChange += HandleInputUserChange;
            anyButtonPressListener.Dispose();
            DisableKeyboardTextInput();

            inputActions.Disable();
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

        #endregion
        
        private static void HandleAnyButtonPressed(InputControl inputControl) => OnAnyButtonPressed?.Invoke();
        private static void HandleTextInput(char c) => OnKeyboardTextInput?.Invoke(c);
        
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
