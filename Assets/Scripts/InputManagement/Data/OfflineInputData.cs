using System.Linq;
using StellarMass.Data;
using StellarMass.InputManagement.Attributes;
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

        [InputContextSelector]
        [SerializeField] private string defaultContext;
        public string DefaultContext => defaultContext;
        
        [SerializeField] private InputContext[] inputContexts;
        public InputContext[] InputContexts => inputContexts;

        private void OnValidate()
        {
            foreach (InputContext inputContext in inputContexts)
            {
                inputContext.EDITOR_SetName(inputContext.Name.AllWhitespaceTrimmed().CapitalizeFirst());
                
                InputActionReference[] inputActionReferences = inputContext.EventSystemActions.AllInputActionReferences;
                foreach (InputActionReference inputActionReference in inputActionReferences)
                {
                    if (inputActionReference == null)
                    {
                        continue;
                    }
                    
                    foreach (string mapName in inputContext.ActiveMaps)
                    {
                        InputActionMap map = inputActionAsset.FindActionMap(mapName);
                        if (map.actions.Contains(inputActionReference.action))
                        {
                            continue;
                        }
                        
                        // NP TODO: Actually prevent the field from being set
                        Debug.LogError($"Action {inputActionReference.action.name} is not a part of the maps in the context {inputContext.Name}! This will not work!");
                        return;
                    }
                }
            }
        }
#endif
    }
}