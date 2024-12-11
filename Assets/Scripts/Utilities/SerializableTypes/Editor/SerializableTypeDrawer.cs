using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Summoner.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.SerializableTypes.Editor
{
    /// <summary>
    /// Code from Nick's personal vault
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableType<>))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private const string FIELD_NAME = "assemblyQualifiedName";
        
        private bool initialized;
        private SerializableType serializableType;
        private readonly List<Type> inheritingTypes = new();

        private void Initialize(SerializedProperty property)
        {
            inheritingTypes.Clear();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            serializableType = EditorInspectorUtility.GetTargetObject<SerializableType>(property);
            Type baseType = ReflectionUtility.GetGenericArgumentForType(serializableType);
            if (baseType == null)
            {
                return;
            }
            
            foreach (Assembly assembly in assemblies)
            {
                inheritingTypes.AddRange(assembly
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && (t.IsSubclassOf(baseType) || t == baseType)));
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialized)
            {
                Initialize(property);
                initialized = true;
            }
            
            SerializedProperty assemblyQualifiedNameProperty = property.FindPropertyRelative(FIELD_NAME);
            Type currentType = Type.GetType(assemblyQualifiedNameProperty.stringValue);
            int currentIndex = Mathf.Max(0, inheritingTypes.IndexOf(currentType));
            
            string[] typeNames = inheritingTypes.ConvertAll(t => t.Name).ToArray();

            EditorGUI.BeginProperty(position, label, property);
            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, typeNames);
            EditorGUI.EndProperty();

            if (newIndex != currentIndex && newIndex < inheritingTypes.Count)
            {
                assemblyQualifiedNameProperty.stringValue = inheritingTypes[newIndex].AssemblyQualifiedName;
            }
        }
    }
}