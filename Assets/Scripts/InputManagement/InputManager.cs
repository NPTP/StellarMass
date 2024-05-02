using System;
using System.Collections.Generic;
using System.Linq;
using StellarMass.InputManagement.MapInstances;
using StellarMass.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

//// NP TODO: Find a way to put the DefaultMap setting into data.
//// NP TODO: Full event system swapping support
//// NP TODO: Fill out null entries in ControlTypeTranslator with correct types
//// NP TODO: Change MARKER to #region for better C# support
//// NP TODO: Change action map enum from enum to property drawer of viable strings based on the input asset.
namespace StellarMass.InputManagement
{
    public static class InputManager
    {
        /// <summary>
        /// This event will always fire on the button press of ANY device, regardless of which
        /// actions maps are enabled or disabled.
        /// </summary>
        public static event Action OnAnyButtonPressed;

        public static event Action OnLastUsedDeviceChanged;

        /// <summary>
        /// Set the map the game will start with, here.
        /// </summary>
        private static MapInstance DefaultMap => Gameplay;

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
            DefaultMap.Enable();
        }

        private static void AddSubscriptions()
        {
            InputSystem.onEvent += HandleEvent;
            InputSystem.onDeviceChange += HandleDeviceChange;
            anyButtonPressListener = InputSystem.onAnyButtonPress.Call(HandleAnyButtonPressed);
        }

        private static void RemoveSubscriptions()
        {
            mapInstances.ForEach(m => m.Terminate());

            InputSystem.onEvent += HandleEvent;
            InputSystem.onDeviceChange += HandleDeviceChange;
            anyButtonPressListener.Dispose();

            inputActions.Disable();
        }

        private static void Terminate()
        {
            RemoveSubscriptions();
        }

        private static void ConfigureEventSystemForMap(MapInstance mapInstance)
        {
            if (!mapInstance.ActionMapEnabled || mapInstance.EventSystemActions == null)
            {
                return;
            }

            InputSystemUIInputModule uiInputModule = UIController.UIInputModule;
            EventSystemActions eventSystemActions = mapInstance.EventSystemActions;

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

        private static void HandleAnyButtonPressed(InputControl inputControl)
        {
            OnAnyButtonPressed?.Invoke();
        }

        private static void HandleDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (lastUsedDevice == inputDevice)
            {
                return;
            }

            lastUsedDevice = inputDevice;
            OnLastUsedDeviceChanged?.Invoke();
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
                // Prevents some devices which spit out noise from triggering our events.
                if (!eventPtr.EnumerateChangedControls(device: device, magnitudeThreshold: 0.0001f).Any())
                    return;
            }

            lastUsedDevice = device;
            OnLastUsedDeviceChanged?.Invoke();
        }

        #region Auto-generated Context Enablers
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

        public static void EnableXContext()
        {
            Gameplay.Disable();
            PauseMenu.Disable();
        }
        // MARKER.ContextEnablers.End
        #endregion
    }
}
