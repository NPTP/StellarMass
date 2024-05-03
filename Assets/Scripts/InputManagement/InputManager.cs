using System;
using System.Collections.Generic;
using System.Linq;
using StellarMass.InputManagement.MapInstances;
using StellarMass.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

//// NP TODO: Full event system swapping support
//// NP TODO: Fill out null entries in ControlTypeTranslator with correct types
//// NP TODO: Define icons & text (with localized strings) for each binding. Serialized dictionary in scriptable
namespace StellarMass.InputManagement
{
    public static class InputManager
    {
        /// <summary>
        /// This event will always fire on the button press of ANY device, regardless of which
        /// actions maps are enabled or disabled.
        /// </summary>
        public static event Action OnAnyButtonPressed;

        public static event Action<char> OnKeyboardTextInput;

        public static event Action<ControlScheme> OnControlSchemeChanged;

        // MARKER.MapInstanceProperties.Start
        public static Gameplay Gameplay { get; private set; }
        public static PauseMenu PauseMenu { get; private set; }
        // MARKER.MapInstanceProperties.End

        private static InputActions inputActions;
        private static List<MapInstance> mapInstances;
        private static IDisposable anyButtonPressListener;
        private static InputDevice lastUsedDevice;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
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
            inputActions = new InputActions();

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

            AddSubscriptions();
            EnableDefaultContext();
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
            mapInstances.ForEach(m => m.Terminate());

            InputSystem.onEvent += HandleEvent;
            InputSystem.onDeviceChange += HandleDeviceChange;
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

        private static void ConfigureEventSystemForContext(InputContext inputContext)
        {
            if (inputContext == null || inputContext.EventSystemActions == null)
            {
                return;
            }

            InputSystemUIInputModule uiInputModule = UIController.UIInputModule;
            EventSystemActions eventSystemActions = inputContext.EventSystemActions;

            uiInputModule.point = eventSystemActions.Point;
            uiInputModule.leftClick = eventSystemActions.LeftClick;
            uiInputModule.middleClick = eventSystemActions.MiddleClick;
            uiInputModule.rightClick = eventSystemActions.RightClick;
            uiInputModule.scrollWheel = eventSystemActions.ScrollWheel;
            uiInputModule.move = eventSystemActions.Move;
            uiInputModule.submit = eventSystemActions.Submit;
            uiInputModule.cancel = eventSystemActions.Cancel;
            uiInputModule.trackedDevicePosition = eventSystemActions.TrackedDevicePosition;
            uiInputModule.trackedDeviceOrientation = eventSystemActions.TrackedDeviceOrientation;
        }

        #region Device/Control Scheme Change Handlers
        private static void HandleDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (lastUsedDevice == inputDevice)
            {
                return;
            }

            lastUsedDevice = inputDevice;
            // NP TODO: Keep or remove?
            // OnControlSchemeChanged?.Invoke();
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
                }
            }
        }

        // Adapted from https://forum.unity.com/threads/detect-most-recent-input-device-type.753206/
        private static void HandleEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (lastUsedDevice == device)
            {
                return;
            }

            if (eventPtr.type == StateEvent.Type)
            {
                // Prevents triggering events for some devices which spit out noise, such as PS4/PS5 gamepads
                if (!eventPtr.EnumerateChangedControls(device: device, magnitudeThreshold: 0.0001f).Any())
                    return;
            }

            lastUsedDevice = device;
            
            // NP TODO: Keep or remove?
            // OnControlSchemeChanged?.Invoke();
        }
        #endregion

        private static ControlScheme? ControlSchemeNameToEnum(string controlSchemeName)
        {
            ControlScheme? controlScheme = null;

            return controlSchemeName switch
            {
                // MARKER.ControlSchemeSwitch.Start
                "MouseKeyboard" => ControlScheme.MouseKeyboard,
                "Gamepad" => ControlScheme.Gamepad,
                // MARKER.ControlSchemeSwitch.End
                _ => null
            };
        }

        #region Auto-generated Context Enablers
        // MARKER.DefaultContextEnabler.Start
        private static void EnableDefaultContext() => EnableGameplayContext();
        // MARKER.DefaultContextEnabler.End
        
        // MARKER.ContextEnablers.Start
        public static void EnableGameplayContext()
        {
            Gameplay.Enable();
            PauseMenu.Disable();
        }

        public static void EnablePauseMenuContext()
        {
            Gameplay.Disable();
            PauseMenu.Enable();
        }
        // MARKER.ContextEnablers.End
        #endregion
    }
}
