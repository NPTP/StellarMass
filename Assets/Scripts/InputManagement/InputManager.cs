using System;
using System.Collections.Generic;
using StellarMass.InputManagement.MapInstances;
using StellarMass.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

//// NP TODO: Find a way to put the DefaultMap setting into data.
//// NP TODO: Full event system swapping support
//// NP TODO: Fill out null entries in ControlTypeTranslator with correct types
//// NP TODO: Support "Context switching": a layer above Action Maps where each context can use several maps together
namespace StellarMass.InputManagement
{
    public static class InputManager
    {
        /// <summary>
        /// This event will always fire on the button press of ANY device, regardless of which
        /// actions maps are enabled or disabled.
        /// </summary>
        public static event Action OnAnyButtonPressed;
        
        /// <summary>
        /// Set the map the game will start with, here.
        /// </summary>
        private static MapInstance DefaultMap => Gameplay;
        
        // MARKER.MapInstanceProperties.Start
        public static Gameplay Gameplay { get; private set; }
        public static PauseMenu PauseMenu { get; private set; }
        // MARKER.MapInstanceProperties.End
        
        private static InputActionsGenerated inputActions;
        private static List<MapInstance> mapInstances;
        private static IDisposable anyButtonPressListener;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;

#else
            Application.quitting -= Terminate;
            Application.quitting += Terminate;
#endif
            
            inputActions = new InputActionsGenerated();

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
            
            mapInstances.ForEach(m => m.OnMapEnabled += HandleMapEnabled);

            DefaultMap.Enable();
            
            anyButtonPressListener = InputSystem.onAnyButtonPress.Call(HandleAnyButtonPressed);
        }

#if UNITY_EDITOR
        private static void HandlePlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange is PlayModeStateChange.ExitingPlayMode) Terminate();
        }
#endif

        private static void Terminate()
        {
            mapInstances.ForEach(m =>
            {
                m.OnMapEnabled -= HandleMapEnabled;
                m.Terminate();
            });
            
            inputActions.Disable();
            anyButtonPressListener.Dispose();
        }

        private static void HandleMapEnabled(MapInstance enabledMap)
        {
            mapInstances.ForEach(m =>
            {
                if (m != enabledMap)
                {
                    m.Disable();
                }
            });
            
            ConfigureEventSystemForMap(enabledMap);
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

        public static void DisableAllMaps()
        {
            mapInstances.ForEach(m => m.Disable());
        }
    }
}
