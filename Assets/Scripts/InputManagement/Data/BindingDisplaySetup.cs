using System;
using UnityEngine;

namespace StellarMass.InputManagement.Data
{
    /// <summary>
    /// Tells us which strings and icons to display for a binding.
    /// </summary>
    [Serializable]
    public class BindingDisplaySetup
    {
        [SerializeField] private string binding; 
        public string Binding => binding;
        
        // NP TODO: Use LocalizedString here instead once package is in
        [SerializeField] private string displayName; 
        public string DisplayName => displayName;
        
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
    }
}