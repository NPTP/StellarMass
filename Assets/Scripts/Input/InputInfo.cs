using System;
using UnityEngine;

namespace StellarMass.Input
{
    [Serializable]
    public class InputInfo
    {
        [SerializeField] private InputMap inputMap;
        public InputMap InputMap => inputMap;

        [SerializeField] private InputType inputType;
        public InputType InputType => inputType;
        
        [SerializeField] private KeyCode[] keyCodes;
        public KeyCode[] KeyCodes => keyCodes;
        
        [SerializeField] private bool ignoreKeyUp;
        public bool IgnoreKeyUp => ignoreKeyUp;
    }
}