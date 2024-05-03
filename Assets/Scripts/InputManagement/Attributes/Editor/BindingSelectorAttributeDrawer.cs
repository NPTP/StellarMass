using System.Linq;
using StellarMass.Utilities.Editor;
using UnityEditor;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(BindingSelectorAttribute))]
    public class BindingSelectorAttributeDrawer : InputNameStringSelectorAttributeDrawer
    {
        protected override string[] GetNames()
        {
            InputActionAsset asset = AssetGetter.GetAsset<InputActionAsset>();
            InputBinding[] bindings = asset.bindings.ToArray();
            string[] names = new string[bindings.Length];
            for (int i = 0; i < bindings.Length; i++)
            {
                names[i] = bindings[i].path;
            }

            return names;
        }
    }
}