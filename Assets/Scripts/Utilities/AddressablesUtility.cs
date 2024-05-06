using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StellarMass.Utilities
{
    public static class AddressablesUtility
    {
        public static T LoadAssetSynchronous<T>(string key)
        {
            T asset;
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
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