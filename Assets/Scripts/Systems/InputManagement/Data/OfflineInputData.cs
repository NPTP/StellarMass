using System.Linq;
using StellarMass.Systems.Data;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace StellarMass.Systems.InputManagement.Data
{
    /// <summary>
    /// Input Data used only in constructing classes from inside the editor. Not meant to be accessed at runtime.
    /// </summary>
    public class OfflineInputData : DataScriptable
    {
#if UNITY_EDITOR
        public const string RUNTIME_INPUT_DATA_ADDRESS = nameof(RuntimeInputData);

        [SerializeField] private bool singlePlayerOnly = true;
        public bool SinglePlayerOnly => singlePlayerOnly;

        [SerializeField] private RuntimeInputData runtimeInputData;
        public RuntimeInputData RuntimeInputData => runtimeInputData;

        [SerializeField] private TextAsset mapActionsTemplateFile;
        public TextAsset MapActionsTemplateFile => mapActionsTemplateFile;
        
        [SerializeField] private TextAsset mapCacheTemplateFile;
        public TextAsset MapCacheTemplateFile => mapCacheTemplateFile;

        [SerializeField] private InputContext defaultContext;
        public InputContext DefaultContext => defaultContext;
        
        [SerializeField] private InputContextInfo[] inputContextInfos;
        public InputContextInfo[] InputContextInfos => inputContextInfos;
        
        private void OnValidate()
        {
            SetAssetAddress(runtimeInputData, RUNTIME_INPUT_DATA_ADDRESS);
            VerifyEventSystemActions();
        }

        private void SetAssetAddress(Object unityObject, string address)
        {
            string path = AssetDatabase.GetAssetPath(unityObject);
            string guid = AssetDatabase.AssetPathToGUID(path);

            AddressableAssetEntry assetEntry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
            if (assetEntry == null)
            {
                assetEntry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
                    guid, AddressableAssetSettingsDefaultObject.Settings.DefaultGroup);
                EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
            }

            assetEntry.address = address;
        }
        
        private void VerifyEventSystemActions()
        {
            foreach (InputContextInfo contextInfo in inputContextInfos)
            {
                contextInfo.EDITOR_SetName(contextInfo.Name.AllWhitespaceTrimmed().CapitalizeFirst());

                InputActionReference[] inputActionReferences = contextInfo.EventSystemActions.AllInputActionReferences;
                foreach (InputActionReference inputActionReference in inputActionReferences)
                {
                    if (inputActionReference == null)
                    {
                        continue;
                    }

                    foreach (string mapName in contextInfo.ActiveMaps)
                    {
                        InputActionMap map = runtimeInputData.InputActionAsset.FindActionMap(mapName);
                        if (map.actions.Contains(inputActionReference.action))
                        {
                            continue;
                        }
                        
                        // NP TODO: Actually prevent the field from being set
                        Debug.LogError($"Action {inputActionReference.action.name} is not a part of the maps in the context {contextInfo.Name}! This will not work!");
                        return;
                    }
                }
            }
        }
#endif
    }
}