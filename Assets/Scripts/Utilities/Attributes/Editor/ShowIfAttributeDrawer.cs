using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        private bool show = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            show = property.serializedObject.targetObject.GetField<bool>(((ShowIfAttribute) attribute).BoolName);

            if (show)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (show)
                return EditorGUI.GetPropertyHeight(property);
            
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}