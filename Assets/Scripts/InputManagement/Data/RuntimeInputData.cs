using System.Collections.Generic;
using System.Linq;
using StellarMass.Data;
using UnityEngine;
using Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StellarMass.InputManagement.Data
{
    /// <summary>
    /// Input Data used at runtime.
    /// </summary>
    [CreateAssetMenu]
    public class RuntimeInputData : DataScriptable
    {
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