using System.Linq;
using StellarMass.Data;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Data
{
    /// <summary>
    /// Input Data used only in constructing classes from inside the editor. Not meant to be accessed at runtime.
    /// </summary>
    [CreateAssetMenu]
    public class OfflineInputData : DataScriptable
    {
#if UNITY_EDITOR
        [SerializeField] private RuntimeInputData runtimeInputData;
        public RuntimeInputData RuntimeInputData => runtimeInputData;

        [SerializeField] private InputActionAsset inputActionAsset;
        public InputActionAsset InputActionAsset => inputActionAsset;

        [SerializeField] private InputContext defaultContext;
        public InputContext DefaultContext => defaultContext;
        
        [SerializeField] private InputContextInfo[] inputContextInfos;
        public InputContextInfo[] InputContextInfos => inputContextInfos;

        private void OnValidate()
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
#endif
    }
}