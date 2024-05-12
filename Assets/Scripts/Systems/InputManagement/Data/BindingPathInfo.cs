using System;
using UnityEngine;

namespace StellarMass.Systems.InputManagement.Data
{
    /// <summary>
    /// Tells us which strings and icons to display for a binding.
    /// </summary>
    [Serializable]
    public class BindingPathInfo
    {
        [SerializeField] private bool overrideDisplayName;
        public bool OverrideDisplayName => overrideDisplayName;

        // NP TODO: Use a localized string here instead!
        [SerializeField] private string displayName;
        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
    }
}