using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Summoner.Utilities.Extensions
{
    public static class AssetReferenceExtensions
    {
        public static T LoadAssetSynchronous<T>(this AssetReference assetReference)
        {
            T asset;
            AsyncOperationHandle<T> handle = assetReference.LoadAssetAsync<T>();
#if UNITY_WEBGL
            // WebGL is single-threaded, and calling WaitForCompletion can lock it, so we use the normal async workflow for this case.
            // See https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/SynchronousAddressables.html
            handle.Completed += onCompleted;
            void onCompleted(AsyncOperationHandle<T> asyncOperationHandle)
            {
                if (asyncOperationHandle.Status is AsyncOperationStatus.Succeeded)
                    asset = asyncOperationHandle.Result;
            }
#else
            asset = handle.WaitForCompletion();
            Addressables.Release(handle);
#endif
            return asset;
        }
    }
}