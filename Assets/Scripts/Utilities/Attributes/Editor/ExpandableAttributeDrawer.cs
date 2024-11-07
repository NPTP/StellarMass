using UnityEngine;
using UnityEditor;

namespace StellarMass.Utilities.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ExpandableScriptable))]
    public class ExpandableScriptableAttributeDrawer : PropertyDrawer
    {
        private static readonly Color backgroundColor = new Color(0.235f, 0.247f, 0.255f);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Store the original label for the top-level field
            GUIContent topLabel = new GUIContent(label);

            // Check if the property is an Object Reference and of ScriptableObject type
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                // Calculate height for background if expanded
                float backgroundHeight = GetPropertyHeight(property, label);
                Rect backgroundRect = new Rect(position.x, position.y, position.width, backgroundHeight - EditorGUIUtility.standardVerticalSpacing);
            
                // Draw the background rectangle
                EditorGUI.DrawRect(backgroundRect, backgroundColor);

                // Draw the ScriptableObject reference field at the top
                Rect objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, topLabel, property.objectReferenceValue, typeof(ScriptableObject), false);

                if (property.objectReferenceValue is ScriptableObject scriptableObject)
                {
                    // Draw the foldout for expanding the ScriptableObject's fields
                    Rect foldoutRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                    property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, "Edit Properties", true);

                    if (property.isExpanded)
                    {
                        EditorGUI.indentLevel++;

                        // Create a SerializedObject to access and edit the internal properties
                        SerializedObject serializedObject = new SerializedObject(scriptableObject);
                        SerializedProperty prop = serializedObject.GetIterator();
                        prop.NextVisible(true); // Skip "m_Script" field

                        float yOffset = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
                        position.y += yOffset;

                        // Draw each property in the ScriptableObject
                        while (prop.NextVisible(false))
                        {
                            position.height = EditorGUI.GetPropertyHeight(prop, true);
                            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height), prop, true);
                            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                        }

                        serializedObject.ApplyModifiedProperties();
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "EditableScriptableObject is only valid for ScriptableObject fields.");
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // If the field is expanded and has a ScriptableObject, calculate additional height to show all fields
            float totalHeight = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded && property.objectReferenceValue is ScriptableObject)
            {
                SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
                SerializedProperty prop = serializedObject.GetIterator();

                prop.NextVisible(true); // Skip "m_Script" field
                while (prop.NextVisible(false))
                {
                    totalHeight += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
                }
                // Additional height for object field and foldout
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                // If not expanded, just add space for the object field
                totalHeight += EditorGUIUtility.singleLineHeight;
            }

            totalHeight += EditorGUIUtility.standardVerticalSpacing;
            return totalHeight;
        }
    }
}