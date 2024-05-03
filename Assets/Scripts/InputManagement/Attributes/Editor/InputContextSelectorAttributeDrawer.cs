using StellarMass.Data;
using StellarMass.Utilities.Editor;
using UnityEditor;

namespace StellarMass.InputManagement.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(InputContextSelectorAttribute))]
    public class InputContextSelectorAttributeDrawer : InputNameStringSelectorAttributeDrawer
    {
        protected override string[] GetNames()
        {
            OfflineInputData offlineInputData = AssetGetter.GetAsset<OfflineInputData>();
            string[] names = new string[offlineInputData.InputContexts.Length];
            for (int i = 0; i < offlineInputData.InputContexts.Length; i++)
            {
                names[i] = offlineInputData.InputContexts[i].Name;
            }

            return names;
        }
    }
}