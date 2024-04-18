using System;
using System.Collections.Generic;
using NPTP.PlayerLoopUtilities;
using StellarMass.Data;

namespace StellarMass.Input
{
    public static class InputReceiver
    {
        public static event Action OnAnyKeyDown;
        
        private static Dictionary<InputType, InputEvent> inputEvents = new();

        private static InputMap activeInputMap = InputMap.Gameplay;
        public static InputMap ActiveInputMap
        {
            get => activeInputMap;
            set
            {
                if (value != activeInputMap)
                {
                    ForceKeysUpForMap(activeInputMap);
                    activeInputMap = value;
                }
            }
        }
        
        public static void Initialize()
        {
            // NP TODO: Don't add every possible input, let it be defined by who subscribes, only
            foreach (InputInfo inputInfo in RTD.InputData.InputInfos)
            {
                inputEvents.Add(inputInfo.InputType, new InputEvent(inputInfo));
            }
            
            PlayerLoopUtility.OnPlayerLoopUpdate += PlayerLoopUpdate;
        }
        
        private static void PlayerLoopUpdate()
        {
            foreach (InputEvent inputEvent in inputEvents.Values)
            {
                if (inputEvent.InputMap != activeInputMap)
                {
                    continue;
                }
                
                inputEvent.PollInput();
            }

            if (UnityEngine.Input.anyKeyDown)
            {
                OnAnyKeyDown?.Invoke();
            }
        }

        public static void AddListeners(InputType inputType, Action keyDownListener, Action keyUpListener = null)
        {
            if (inputEvents.TryGetValue(inputType, out InputEvent inputEvent))
            {
                inputEvent.AddListeners(keyDownListener, keyUpListener);
            }
        }

        public static void RemoveListeners(InputType inputType, Action keyDownListener, Action keyUpListener = null)
        {
            if (inputEvents.TryGetValue(inputType, out InputEvent inputEvent))
            {
                inputEvent.RemoveListeners(keyDownListener, keyUpListener);
            }
        }

        public static bool GetKeyDown(InputType inputType)
        {
            return inputEvents.TryGetValue(inputType, out InputEvent inputEvent) && inputEvent.KeyDown;
        }
        
        private static void ForceKeysUpForMap(InputMap inputMap)
        {
            foreach (InputEvent inputEvent in inputEvents.Values)
            {
                if (inputEvent.InputMap != inputMap)
                {
                    continue;
                }

                inputEvent.ForceKeyUp();
            }
        }
    }
}