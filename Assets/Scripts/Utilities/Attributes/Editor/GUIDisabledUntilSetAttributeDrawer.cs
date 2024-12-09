using UnityEditor;

namespace Summoner.Utilities.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(GUIDisabledUntilSetAttribute))]
    public class GUIDisabledUntilSetAttributeDrawer : GUIDisabledAttributeDrawer
    {
        protected override bool DisabledCondition(SerializedProperty property)
        {
            return property.objectReferenceValue != null;
        }
    }
}