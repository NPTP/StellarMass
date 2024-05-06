using System;
using StellarMass.InputManagement.Attributes;
using UnityEngine;

namespace StellarMass.InputManagement
{
    [Serializable]
    public class InputContextInfo
    {
        [SerializeField] private string name;
        public string Name => name;

        [InputMapSelector]
        [SerializeField] private string[] activeMaps;
        public string[] ActiveMaps => activeMaps;

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