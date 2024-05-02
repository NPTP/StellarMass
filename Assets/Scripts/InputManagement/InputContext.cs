using System;
using UnityEngine;

namespace StellarMass.InputManagement
{
    [Serializable]
    public class InputContext
    {
        [SerializeField] private string name;
        public string Name => name;

        [SerializeField] private string[] activeMaps;
        public string[] ActiveMaps => activeMaps;
        
#if UNITY_EDITOR
        public void EDITOR_SetName(string n)
        {
            name = n;
        }
#endif
    }
}