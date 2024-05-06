using UnityEngine;
using Utilities;

namespace StellarMass.InputManagement.Data
{
    /// <summary>
    /// Contains all of the binding data for a particular input method/device: e.g. Mouse+Kbd, Xbox gamepad, etc. 
    /// </summary>
    [CreateAssetMenu]
    public class BindingData : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<string, BindingPathInfo> bindingDisplayInfo = new();
        public SerializableDictionary<string, BindingPathInfo> BindingDisplayInfo => bindingDisplayInfo;
    }
}