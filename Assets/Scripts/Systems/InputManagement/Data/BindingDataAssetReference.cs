using System;
using UnityEngine.AddressableAssets;

namespace StellarMass.Systems.InputManagement.Data
{
    [Serializable]
    public class BindingDataAssetReference : AssetReferenceT<BindingData>
    {
        protected BindingDataAssetReference(string guid) : base(guid)
        {
        }
    }
}