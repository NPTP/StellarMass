using System;
using System.Collections.Generic;
using UnityEngine;

namespace StellarMass.Input
{
    public class InputReceiver : MonoBehaviour
    {
        private static Dictionary<KeyCode, InputEvent> inputEvents = new();

        private void Awake()
        {
            foreach (KeyCode keyCode in Inputs.AllInputs)
            {
                inputEvents.Add(keyCode, new InputEvent(keyCode));
            }
        }

        public static void AddListeners(KeyCode keyCode, Action keyDownListener, Action keyUpListener = null)
        {
            if (inputEvents.TryGetValue(keyCode, out InputEvent inputEvent))
            {
                inputEvent.AddListeners(keyDownListener, keyUpListener);
            }
        }

        public static void RemoveListeners(KeyCode keyCode, Action keyDownListener, Action keyUpListener = null)
        {
            if (inputEvents.TryGetValue(keyCode, out InputEvent inputEvent))
            {
                inputEvent.RemoveListeners(keyDownListener, keyUpListener);
            }
        }

        public static bool GetKeyDown(KeyCode keyCode)
        {
            return inputEvents.TryGetValue(keyCode, out InputEvent inputEvent) && inputEvent.KeyDown;
        }

        private void Update()
        {
            foreach (InputEvent inputEvent in inputEvents.Values)
            {
                inputEvent.PollInput();
            }
        }
    }
}