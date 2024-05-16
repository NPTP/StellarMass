using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

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
        
#if UNITY_EDITOR
        public static void EDITOR_SetAddress(this Object unityObject, string address, bool createIfNotAddressable = false)
        {
            AddressableAssetEntry entry = createIfNotAddressable ? EDITOR_AddObjectToAddressablesDefaultSettings(unityObject) : EDITOR_GetEntry(unityObject);
            entry.address = address;
        }
        
        public static AddressableAssetEntry EDITOR_AddObjectToAddressablesDefaultSettings(Object unityObject)
        {
            AddressableAssetEntry assetEntry = EDITOR_GetEntry(unityObject);
            if (assetEntry == null)
            {
                assetEntry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
                    AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(unityObject)),
                    AddressableAssetSettingsDefaultObject.Settings.DefaultGroup);
                EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
            }

            return assetEntry;
        }

        public static AddressableAssetEntry EDITOR_GetEntry(Object unityObject)
        {
            string path = AssetDatabase.GetAssetPath(unityObject);
            string guid = AssetDatabase.AssetPathToGUID(path);

            return AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
        }
#endif
    }
}