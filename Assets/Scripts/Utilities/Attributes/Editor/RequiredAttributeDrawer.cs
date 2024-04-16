using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StellarMass.Utilities.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.ObjectField(position, property, label);
            SetRequiredComponentReference(property, fieldInfo);
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndProperty();
        }

        private static void SetRequiredComponentReference(SerializedProperty property, FieldInfo fieldInfo)
        {
            Component target = property.serializedObject.targetObject as Component;

            if (property.serializedObject.targetObject is Component componentField)
            {
                if (property.isArray)
                {
                    Component[] components = componentField.GetComponents(fieldInfo.FieldType);
                    property.arraySize = components.Length;
                    for (int i = 0; i < components.Length; i++)
                    {
                        property.GetArrayElementAtIndex(i).objectReferenceValue = components[i];
                    }
                }
                else
                {
                    Component componentObject = property.objectReferenceValue as Component;
                    if (componentObject == null || componentObject.gameObject != target.gameObject)
                    {
                        Component component = target.GetComponent(fieldInfo.FieldType);
                        if (component == null)
                        {
                            component = target.gameObject.AddComponent(fieldInfo.FieldType);
                        }
                        property.objectReferenceValue = component;
                    }
                }
            }
        }
    }
}