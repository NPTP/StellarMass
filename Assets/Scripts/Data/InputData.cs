using StellarMass.InputManagement;
using StellarMass.Utilities.Extensions;
using UnityEngine;

namespace StellarMass.Data
{
    [CreateAssetMenu]
    public class InputData : DataScriptable
    {
        [SerializeField] private InputContext[] actionContexts;
        public InputContext[] ActionContexts => actionContexts;

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < actionContexts.Length; i++)
            {
                InputContext inputContext = actionContexts[i];
                inputContext.EDITOR_SetName(inputContext.Name.AllWhitespaceTrimmed().CapitalizeFirst());
            }
        }
#endif
    }
}