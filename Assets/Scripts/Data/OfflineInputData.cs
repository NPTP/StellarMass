using System;
using System.Reflection;
using StellarMass.InputManagement;
using StellarMass.InputManagement.Attributes;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace StellarMass.Data
{
    /// <summary>
    /// Input Data used only in constructing classes from inside the editor. Not meant to be accessed at runtime.
    /// </summary>
    [CreateAssetMenu]
    public class OfflineInputData : DataScriptable
    {
        [InputContextSelector]
        [SerializeField] private string defaultContext;
        public string DefaultContext => defaultContext;
        
        [FormerlySerializedAs("actionContexts")] [SerializeField] private InputContext[] inputContexts;
        public InputContext[] InputContexts => inputContexts;

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (InputContext inputContext in inputContexts)
            {
                inputContext.EDITOR_SetName(inputContext.Name.AllWhitespaceTrimmed().CapitalizeFirst());
                
                object eventSystemActions = inputContext.EventSystemActions;
                Type type = eventSystemActions.GetType();

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                {
                    if (!Attribute.IsDefined(field, typeof(SerializeField)))
                    {
                        continue;
                    }
                    
                    object value = field.GetValue(eventSystemActions);
                    if (value is InputActionReference inputActionReference)
                    {
                        // NP TODO: Check that the value of the action is valid for the maps in the context. O(n^2) check
                    }
                }
            }
        }
#endif
    }
}