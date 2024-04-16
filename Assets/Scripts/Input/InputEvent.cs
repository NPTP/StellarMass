using System;
using UnityEngine;

namespace StellarMass.Input
{
    public class InputEvent
    {
        public event Action OnInputDown;
        public event Action OnInputUp;
        
        public KeyCode KeyCode { get; }
        
        public bool KeyDown { get; private set; }

        public InputEvent(KeyCode keyCode)
        {
            KeyCode = keyCode;
        }

        public void AddListeners(Action keyDownListener, Action keyUpListener)
        {
            if (keyDownListener != null) OnInputDown += keyDownListener;
            if (keyUpListener != null) OnInputUp += keyUpListener;
        }
        
        public void RemoveListeners(Action keyDownListener, Action keyUpListener)
        {
            if (keyDownListener != null) OnInputDown -= keyDownListener;
            if (keyUpListener != null) OnInputUp -= keyUpListener;
        }

        public void PollInput()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode))
            {
                KeyDown = true;
                OnInputDown?.Invoke();
            }
            else if (UnityEngine.Input.GetKeyUp(KeyCode))
            {
                KeyDown = false;
                OnInputUp?.Invoke();
            }
        }
    }
}