using System.Linq;
using StellarMass.Data;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace StellarMass.InputManagement.Data
{
    /// <summary>
    /// Input Data used only in constructing classes from inside the editor. Not meant to be accessed at runtime.
    /// </summary>
    [CreateAssetMenu]
    public class OfflineInputData : DataScriptable
    {
#if UNITY_EDITOR
        public const string RUNTIME_INPUT_DATA_ADDRESS = nameof(RuntimeInputData);
        
        [SerializeField] private RuntimeInputData runtimeInputData;

        [SerializeField] private InputActionAsset inputActionAsset;
        public InputActionAsset InputActionAsset => inputActionAsset;

        [SerializeField] private InputContext defaultContext;
        public InputContext DefaultContext => defaultContext;
        
        [SerializeField] private InputContextInfo[] inputContextInfos;
        public InputContextInfo[] InputContextInfos => inputContextInfos;

        private void OnValidate()
        {
            SetRuntimeInputDataAddress();

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
                        InputActionMap map = inputActionAsset.FindActionMap(mapName);
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

        private void SetRuntimeInputDataAddress()
        {
            string path = AssetDatabase.GetAssetPath(runtimeInputData);
            string guid = AssetDatabase.AssetPathToGUID(path);

            AddressableAssetEntry runtimeInputAssetEntry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
            if (runtimeInputAssetEntry == null)
            {
                runtimeInputAssetEntry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
                    guid, AddressableAssetSettingsDefaultObject.Settings.DefaultGroup);
                EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
            }

            runtimeInputAssetEntry.address = RUNTIME_INPUT_DATA_ADDRESS;
        }
#endif
    }
}