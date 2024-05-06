using System;
using System.Collections.Generic;
using StellarMass.Data;
using UnityEngine;

namespace StellarMass.InputManagement.Data
{
    /// <summary>
    /// Input Data used at runtime.
    /// </summary>
    [CreateAssetMenu]
    public class RuntimeInputData : DataScriptable
    {
        [SerializeField] private bool useContextEventSystemActions;
        public bool UseContextEventSystemActions => useContextEventSystemActions;
        
        [SerializeField] private BindingDisplaySetup[] bindingDisplaySetups = Array.Empty<BindingDisplaySetup>();
        public Dictionary<string, BindingDisplaySetup> BindingDisplaySetups
        {
            get
            {
                Dictionary<string, BindingDisplaySetup> dict = new();

                foreach (BindingDisplaySetup bindingDisplaySetup in bindingDisplaySetups)
                {
                    if (string.IsNullOrEmpty(bindingDisplaySetup.Binding))
                    {
                        continue;
                    }

                    dict.TryAdd(bindingDisplaySetup.Binding, bindingDisplaySetup);
                }

                return dict;
            }
        }
    }
}