using System;
using StellarMass.Systems.InputManagement.Data;
using StellarMass.Systems.InputManagement.Generated.MapActions;
using StellarMass.Utilities;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        
        // This event will invoke regardless of contexts/maps being enabled/disabled.
        public static event Action OnAnyButtonPressed;

        private static InputPlayerCollection playerCollection;
        
        // MARKER.PlayerGetter.Start
        private static InputPlayer Player(int id) => playerCollection.GetPlayer(id);
        // MARKER.PlayerGetter.End
        
        // MARKER.SinglePlayerProperties.Start
        public static GameplayActions Gameplay => Player(0).Gameplay;
        public static PauseMenuActions PauseMenu => Player(0).PauseMenu;
        public static InputContext CurrentContext => Player(0).CurrentContext;
        public static ControlScheme CurrentControlScheme => Player(0).CurrentControlScheme;
        public static void EnableContext(InputContext context) => Player(0).EnableContext(context);
        // MARKER.SinglePlayerProperties.End
        
        private static RuntimeInputData runtimeInputData;
        private static PlayerInput primaryPlayerInput;
        private static InputSystemUIInputModule primaryUIInputModule;
        private static IDisposable anyButtonPressListener;
        
        public static bool AllowPlayerJoining { get; set; }
        public static bool UseContextEventSystemActions => runtimeInputData.UseContextEventSystemActions;
        
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

            playerCollection = new InputPlayerCollection(asset);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            primaryPlayerInput = Object.FindObjectOfType<PlayerInput>();
            primaryUIInputModule = Object.FindObjectOfType<InputSystemUIInputModule>();

            if (primaryPlayerInput == null || primaryUIInputModule == null)
            {
                GameObject inputMgmtGameObject = new GameObject("Player [0] Input Management");
                if (primaryPlayerInput == null)
                    primaryPlayerInput = inputMgmtGameObject.AddComponent<PlayerInput>();
                if (primaryUIInputModule == null)
                    primaryUIInputModule = inputMgmtGameObject.AddComponent<InputSystemUIInputModule>();
                Object.DontDestroyOnLoad(inputMgmtGameObject);
            }
            
            primaryPlayerInput.actions = runtimeInputData.InputActionAsset;

            InputPlayer primaryPlayer = Player(0);
            primaryPlayer.PlayerInput = primaryPlayerInput;
            primaryPlayer.UIInputModule = primaryUIInputModule;
            primaryPlayerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            
            playerCollection.ForEach(p => p.EnableContext(DefaultContext));
            AddSubscriptions();
        }

        private static void Terminate()
        {
            RemoveSubscriptions();
        }

        private static void AddSubscriptions()
        {
            anyButtonPressListener = InputSystem.onAnyButtonPress.Call(HandleAnyButtonPressed);
        }

        private static void RemoveSubscriptions()
        {
            anyButtonPressListener.Dispose();
            playerCollection.ForEach(p => p.Terminate());
        }

        #endregion

        #region Public Interface
        
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
            
            playerCollection.ForEach(p => p.FindActionEventAndSubscribe(actionReference, callback, subscribe));
        }
        
        #endregion

        #region Private Runtime Functionality

        private static void ParseInputControlPath(InputControl inputControl, out string deviceName, out string controlPath)
        {
            deviceName = inputControl.device.name;
            controlPath = inputControl.path[(2 + deviceName.Length)..];
        }

        private static void HandleAnyButtonPressed(InputControl inputControl)
        {
            OnAnyButtonPressed?.Invoke();

            if (!AllowPlayerJoining)
            {
                return;
            }

            InputDevice device = inputControl.device;

            // Ignore presses on devices that are already used by a player.
            if (PlayerInput.FindFirstPairedToDevice(device) != null)
            {
                return;
            }

            // Create a new player. (If the player did not end up with a valid input setup, it will return null)
            PlayerInput playerInput = PlayerInput.Instantiate(primaryPlayerInput.gameObject, pairWithDevice: device);
            if (playerInput == null)
            {
                return;
            }
            
            InputPlayer newPlayer = playerCollection.AddPlayer(playerInput.actions);
            GameObject playerInputGameObject = playerInput.gameObject;
            playerInputGameObject.name = $"Player [{newPlayer.ID}] Input Management";
            newPlayer.PlayerInput = playerInput;
            newPlayer.UIInputModule = playerInputGameObject.AddComponent<InputSystemUIInputModule>();
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            Object.DontDestroyOnLoad(playerInputGameObject);
        }

        #endregion
    }
}
