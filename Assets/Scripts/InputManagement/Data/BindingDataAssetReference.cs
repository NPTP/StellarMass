using System;
using UnityEngine.AddressableAssets;

namespace StellarMass.InputManagement.Data
{
    [Serializable]
    public class BindingDataAssetReference : AssetReferenceT<BindingData>
    {
        public BindingDataAssetReference(string guid) : base(guid)
        {
        }
    }
}