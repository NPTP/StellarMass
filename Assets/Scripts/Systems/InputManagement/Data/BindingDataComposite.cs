using StellarMass.Utilities;
using UnityEngine;

namespace StellarMass.Systems.InputManagement.Data
{
    /// <summary>
    /// For multi-device binding data that should be paired (e.g. Mouse + Keyboard)
    /// </summary>
    [CreateAssetMenu]
    public sealed class BindingDataComposite : BindingData
    {
        [SerializeField] private BindingDataSingle[] deviceBindingData;
        public override SerializableDictionary<string, BindingPathInfo> BindingDisplayInfo
        {
            get
            {
                if (deviceBindingData == null || deviceBindingData.Length == 0)
                {
                    return null;
                }

                SerializableDictionary<string, BindingPathInfo> returnValue = new();
                foreach (BindingDataSingle info in deviceBindingData)
                {
                    returnValue.AddRange(info.BindingDisplayInfo);
                }

                return returnValue;
            }
        }
    }
}