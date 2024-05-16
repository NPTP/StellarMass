using StellarMass.Systems.Data;
using StellarMass.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.Systems.InputManagement.Data
{
    /// <summary>
    /// Input Data used at runtime.
    /// </summary>
    [CreateAssetMenu]
    public class RuntimeInputData : DataScriptable
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        public InputActionAsset InputActionAsset => inputActionAsset;

        [SerializeField] private bool useContextEventSystemActions;
        public bool UseContextEventSystemActions => useContextEventSystemActions;

        [SerializeField] private SerializableDictionary<ControlScheme, BindingDataAssetReference> bindingDataReferences;
        public SerializableDictionary<ControlScheme, BindingDataAssetReference> BindingDataReferences => bindingDataReferences;
    }
}