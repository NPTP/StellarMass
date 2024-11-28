using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Summoner.Systems.AssetLoading
{
    public class LoadAsset<T> : CustomYieldInstruction where T : UnityEngine.Object
    {
        public override bool keepWaiting => !completed;

        private bool completed;
        
        public LoadAsset(AssetReferenceT<T> assetReference, Action<Asset<T>> loadCompletedCallback)
        {
            AssetLoader.Load(assetReference, asset =>
            {
                completed = true;
                loadCompletedCallback?.Invoke(asset);
            });
        }

        public LoadAsset(string address, Action<Asset<T>> loadCompletedCallback)
        {
            AssetLoader.Load<T>(address, asset =>
            {
                completed = true;
                loadCompletedCallback?.Invoke(asset);
            });
        }
    }
}
