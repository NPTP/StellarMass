using System;
using System.Collections.Generic;
using StellarMass.InputManagement.MapInstances;
using StellarMass.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;

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
        /// NP TODO: Find a way to put this setting into data.
        /// </summary>
        private static MapInstance DefaultMap => Gameplay;
        
        // MARKER.MapInstanceProperties.Start
        public static Gameplay Gameplay { get; private set; }
        public static PauseMenu PauseMenu { get; private set; }
        // MARKER.MapInstanceProperties.End
        
        private static InputActions inputActions;
        private static List<MapInstance> mapInstances;
        private static IDisposable anyButtonPressListener;
        
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
                PauseMenu
                // MARKER.CollectionInitializer.End
            };
            
            ExecuteOnAllMapInstances(m => m.OnMapEnabled += HandleMapEnabled);

            DefaultMap.Enable();
            
            anyButtonPressListener = InputSystem.onAnyButtonPress.Call(HandleAnyButtonPressed);
        }

        private static void HandleMapEnabled(MapInstance enabledMap)
        {
            ExecuteOnAllMapInstances(m =>
            {
                if (m != enabledMap)
                {
                    m.Disable();
                }
            });
            
            ConfigureEventSystemForMap(enabledMap);
        }
        
        // NP TODO: Full event system swapping support
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

        private static void Terminate()
        {
            ExecuteOnAllMapInstances(m =>
            {
                m.OnMapEnabled -= HandleMapEnabled;
                m.Terminate();
            });
            
            inputActions.Disable();
            anyButtonPressListener.Dispose();
        }

        public static void DisableAllMaps()
        {
            ExecuteOnAllMapInstances(m => m.Disable());
        }

        private static void ExecuteOnAllMapInstances(Action<MapInstance> action)
        {
            foreach (MapInstance mapInstance in mapInstances)
            {
                action?.Invoke(mapInstance);
            }
        }
    }
}