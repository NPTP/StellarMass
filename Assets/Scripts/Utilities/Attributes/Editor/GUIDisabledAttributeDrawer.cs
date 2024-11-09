using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(GUIDisabledAttribute))]
    public class GUIDisabledAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndProperty();
        }
    }
}