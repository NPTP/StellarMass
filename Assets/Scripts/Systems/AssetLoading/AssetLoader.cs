using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Summoner.Systems.AssetLoading
{
    /// <summary>
    /// Addressables asset loading wrapper
    /// </summary>
    public static class AssetLoader
    {
        public static void Load<T>(AssetReferenceT<T> assetReference, Action<Asset<T>> loadCompletedCallback) where T : Object
        {
            ProcessAsyncOperationHandle(assetReference.LoadAssetAsync(), loadCompletedCallback);
        }
        
        public static void Load<T>(string address, Action<Asset<T>> loadCompletedCallback) where T : Object
        {
            ProcessAsyncOperationHandle(Addressables.LoadAssetAsync<T>(address), loadCompletedCallback);
        }

        private static void ProcessAsyncOperationHandle<T>(AsyncOperationHandle<T> handle, Action<Asset<T>> loadCompletedCallback) where T : Object
        {
            handle.Completed += onHandleOnCompleted;
            
            void onHandleOnCompleted(AsyncOperationHandle<T> _)
            {
                handle.Completed -= onHandleOnCompleted;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    loadCompletedCallback?.Invoke(new Asset<T>(handle));
                }
            }
        }
    }
}
