using System;
using UnityEngine;

namespace StellarMass.Input
{
    public class InputEvent
    {
        public event Action OnKeyDown;
        public event Action OnKeyUp;

        public bool KeyDown { get; private set; }
        public InputMap InputMap { get; }
        public bool StopFollowingInputsOnFrame { get; }
        private KeyCode[] KeyCodes { get; }
        private bool IgnoreKeyUp { get; }

        public InputEvent(InputInfo inputInfo)
        {
            InputMap = inputInfo.InputMap;
            KeyCodes = inputInfo.KeyCodes;
            IgnoreKeyUp = inputInfo.IgnoreKeyUp;
            StopFollowingInputsOnFrame = inputInfo.StopFollowingInputsOnFrame;
        }
        
        public void AddListeners(Action keyDownListener, Action keyUpListener)
        {
            if (keyDownListener != null) OnKeyDown += keyDownListener;
            if (keyUpListener != null) OnKeyUp += keyUpListener;
        }
        
        public void RemoveListeners(Action keyDownListener, Action keyUpListener)
        {
            if (keyDownListener != null) OnKeyDown -= keyDownListener;
            if (keyUpListener != null) OnKeyUp -= keyUpListener;
        }

        public void ForceKeyUp()
        {
            if (!IgnoreKeyUp)
            {
                KeyDown = false;
                OnKeyUp?.Invoke();
            }
        }

        /// <summary>
        /// Return true if an input (key down or up) was received.
        /// </summary>
        public bool PollInput()
        {
            for (int i = 0; i < KeyCodes.Length; i++)
            {
                KeyCode keyCode = KeyCodes[i];
                if (UnityEngine.Input.GetKeyDown(keyCode))
                {
                    KeyDown = true;
                    OnKeyDown?.Invoke();
                    return true;
                }
                else if (UnityEngine.Input.GetKeyUp(keyCode))
                {
                    KeyDown = false;
                    if (!IgnoreKeyUp)
                    {
                        OnKeyUp?.Invoke();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}