using UnityEditor;
using UnityEngine;

namespace StellarMass.InputManagement.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ActionReferenceWrapper))]
    public class ActionReferenceWrapperDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty referenceProperty = property.FindPropertyRelative("reference");
            EditorGUI.PropertyField(position, referenceProperty, new GUIContent("Input Action Reference"));
            EditorGUI.EndProperty();
        }
    }
}