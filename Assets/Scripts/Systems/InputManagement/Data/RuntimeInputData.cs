using System.Collections.Generic;
using System.Linq;
using StellarMass.Systems.Data;
using StellarMass.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StellarMass.Systems.InputManagement.Data
{
    /// <summary>
    /// Input Data used at runtime.
    /// </summary>
    [CreateAssetMenu]
    public class RuntimeInputData : DataScriptable
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        public InputActionAsset InputActionAsset => inputActionAsset;

        [SerializeField] private bool useContextEventSystemActions;
        public bool UseContextEventSystemActions => useContextEventSystemActions;

        [SerializeField] private SerializableDictionary<string, BindingDataAssetReference> bindingDataReferences;
        public SerializableDictionary<string, BindingDataAssetReference> BindingDataReferences => bindingDataReferences;

        private void OnValidate()
        {
#if UNITY_EDITOR
            List<BindingDataAssetReference> values = bindingDataReferences.Values.ToList();

            bool dirty = false;
            
            foreach (BindingDataAssetReference value in values)
            {
                if (value == null)
                {
                    continue;
                }

                bindingDataReferences.EDITOR_SetKey(value, value.editorAsset.name);
                dirty = true;
            }

            if (dirty) EditorUtility.SetDirty(this);
#endif
        }
    }
}