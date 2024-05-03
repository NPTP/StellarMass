using UnityEditor;
using UnityEngine;

namespace StellarMass.InputManagement.Attributes.Editor
{
    public abstract class InputNameStringSelectorAttributeDrawer : PropertyDrawer
    {
        private bool hasInitialized;
        private string[] names;
        
        protected abstract string[] GetNames();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!hasInitialized)
            {
                hasInitialized = true;
                names = GetNames();
            }
            
            if (property.propertyType == SerializedPropertyType.String)
            {
                int index = Mathf.Max(0, System.Array.IndexOf(names, property.stringValue));
                EditorGUI.BeginProperty(position, label, property);
                index = EditorGUI.Popup(position, label.text, index, names);
                property.stringValue = names[index];
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}