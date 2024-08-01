using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif

namespace StellarMass.Utilities
{
    public static class AddressablesUtility
    {
        public static T LoadAssetSynchronous<T>(string key)
        {
#if UNITY_WEBGL
            // WebGL is single-threaded, and calling WaitForCompletion can lock it, so we use the normal async workflow for this case.
            // See https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/SynchronousAddressables.html
            return default;
#endif
            T asset;
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            asset = handle.WaitForCompletion();
            Addressables.Release(handle);
            return asset;
        }

        public static void LoadAssetAsync<T>(string key, Action<T> completedHandler)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            handle.Completed += operationHandle =>
            {
                if (operationHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    return;
                }

                completedHandler?.Invoke(operationHandle.Result);
                Addressables.Release(handle);
            };
        }

#if UNITY_EDITOR
        private static string EDITOR_ToGuid(Object unityObject) => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(unityObject));

        public static AddressableAssetEntry EDITOR_AddObjectToAddressablesGroup(Object unityObject, string groupName)
        {
            return EDITOR_AddObjectToAddressablesGroupInternal(unityObject, useDefaultGroup: false, groupName);
        }
        
        public static AddressableAssetEntry EDITOR_AddObjectToAddressablesDefaultGroup(Object unityObject)
        {
            return EDITOR_AddObjectToAddressablesGroupInternal(unityObject, useDefaultGroup: true);
        }

        private static AddressableAssetEntry EDITOR_AddObjectToAddressablesGroupInternal(Object unityObject, bool useDefaultGroup, string groupName = "")
        {
            if (EDITOR_TryGetEntry(unityObject, out AddressableAssetEntry entry))
            {
                return entry;
            }
            
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.DefaultGroup;
                
            if (!useDefaultGroup && !string.IsNullOrEmpty(groupName))
            {
                group = settings.FindGroup(groupName);
                if (group == null)
                {
                    group = settings.CreateGroup(groupName, true, false, false, null,
                        typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
                }
            }

            entry = settings.CreateOrMoveEntry(EDITOR_ToGuid(unityObject), group, false, false);

            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true, false);
            
            return entry;
        }

        public static void EDITOR_RemoveEntry(Object unityObject)
        {
            AddressableAssetSettingsDefaultObject.Settings.RemoveAssetEntry(EDITOR_ToGuid(unityObject));
        }

        public static bool EDITOR_TryGetEntry(Object unityObject, out AddressableAssetEntry entry)
        {
            entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(EDITOR_ToGuid(unityObject));
            return entry != null;
        }

        public static void EDITOR_SetAddress(this Object unityObject, string address, bool createIfNotAddressable = false)
        {
            if (EDITOR_TryGetEntry(unityObject, out AddressableAssetEntry entry))
            {
                entry.address = address;
            }
        }
#endif
    }
}