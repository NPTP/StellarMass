﻿using System;
using StellarMass.Systems.InputManagement.Attributes;
using UnityEngine;

namespace StellarMass.Systems.InputManagement.Data
{
    [Serializable]
    public class InputContextInfo
    {
        [SerializeField] private string name;
        public string Name => name;

        [InputMapSelector]
        [SerializeField] private string[] activeMaps;
        public string[] ActiveMaps => activeMaps;

        [SerializeField] private bool enableKeyboardTextInput;
        public bool EnableKeyboardTextInput => enableKeyboardTextInput;

        [SerializeField] private EventSystemActions eventSystemActions;
        public EventSystemActions EventSystemActions => eventSystemActions;
        
#if UNITY_EDITOR
        public void EDITOR_SetName(string n)
        {
            name = n;
        }
#endif
    }
}