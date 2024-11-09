using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.SerializableTypes.Editor
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        // List of types you want to be selectable in the dropdown
        private List<Type> availableTypes = new List<Type>
        {
            typeof(int),
            typeof(float),
            typeof(string),
            typeof(GameObject),
            typeof(Transform)
            // Add more types as needed
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get the type name property and current type
            SerializedProperty typeNameProp = property.FindPropertyRelative("typeName");
            Type currentType = Type.GetType(typeNameProp.stringValue);

            // Find the index of the current type in the availableTypes list
            int currentIndex = availableTypes.IndexOf(currentType);
            if (currentIndex == -1) currentIndex = 0; // Default to the first type if not found

            // Convert available types to an array of names for display
            string[] typeNames = availableTypes.ConvertAll(t => t.Name).ToArray();

            // Draw the label and dropdown
            EditorGUI.BeginProperty(position, label, property);
            currentIndex = EditorGUI.Popup(position, label.text, currentIndex, typeNames);
            EditorGUI.EndProperty();

            // Update the type name property if a new type is selected
            if (currentIndex >= 0 && currentIndex < availableTypes.Count)
            {
                typeNameProp.stringValue = availableTypes[currentIndex].AssemblyQualifiedName;
            }
        }
    }
}