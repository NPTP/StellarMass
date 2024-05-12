using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
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
        
        public static void SetAddress(this Object unityObject, string address, bool createIfNotAddressable = false)
        {
            AddressableAssetEntry entry = createIfNotAddressable ? AddObjectToAddressablesDefaultSettings(unityObject) : GetEntry(unityObject);
            entry.address = address;
        }
        
        public static AddressableAssetEntry AddObjectToAddressablesDefaultSettings(Object unityObject)
        {
            AddressableAssetEntry assetEntry = GetEntry(unityObject);
            if (assetEntry == null)
            {
                assetEntry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
                    AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(unityObject)),
                    AddressableAssetSettingsDefaultObject.Settings.DefaultGroup);
                EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
            }

            return assetEntry;
        }

        public static AddressableAssetEntry GetEntry(Object unityObject)
        {
            string path = AssetDatabase.GetAssetPath(unityObject);
            string guid = AssetDatabase.AssetPathToGUID(path);

            return AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
        }
    }
}