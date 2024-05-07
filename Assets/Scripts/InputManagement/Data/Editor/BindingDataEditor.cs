using System;
using StellarMass.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace StellarMass.InputManagement.Data.Editor
{
    [CustomEditor(typeof(BindingData))]
    public class BindingDataEditor : UnityEditor.Editor
    {
        private const string MOUSE = "Generate Mouse Control Paths";
        private const string KEYBOARD = "Generate Keyboard Control Paths";
        private const string GAMEPAD = "Generate Gamepad Control Paths";
        
        private BindingData targetBindingData;

        private void OnEnable()
        {
            targetBindingData = (BindingData)target;
        }

        public override void OnInspectorGUI()
        {
            GeneratorButton(MOUSE);
            GeneratorButton(KEYBOARD);
            GeneratorButton(GAMEPAD);
            
            EditorInspectorUtility.DrawHorizontalLine();

            SerializedProperty dictProperty = serializedObject.FindProperty("bindingDisplayInfo");
            SerializedProperty listProperty = dictProperty.FindPropertyRelative("keyValueCombos");

            for (int i = 0; i < listProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                SerializedProperty keyProperty = elementProperty.FindPropertyRelative("key");
                SerializedProperty valueProperty = elementProperty.FindPropertyRelative("value");
                
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(keyProperty, true);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(valueProperty, true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                
                if (GUILayout.Button($"Delete <{keyProperty.stringValue}>"))
                {
                    targetBindingData.BindingDisplayInfo.EDITOR_Remove(i);
                    EditorUtility.SetDirty(targetBindingData);
                    serializedObject.Update();
                    break;
                }

                EditorInspectorUtility.DrawHorizontalLine();
            }

            if (GUILayout.Button("Add New Entry"))
            {
                targetBindingData.BindingDisplayInfo.EDITOR_Add(string.Empty, new BindingPathInfo());
                EditorUtility.SetDirty(targetBindingData);
                serializedObject.Update();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void GeneratorButton(string label)
        {
            if (!GUILayout.Button(label))
            {
                return;
            }

            if (!EditorUtility.DisplayDialog(label, $"Are you sure? This will overwrite any existing entries.", label, "Cancel"))
            {
                return;
            }
            
            string[] controls = Array.Empty<string>();
            switch (label)
            {
                case MOUSE:
                    controls = InputDataHelper.MouseControls;
                    break;
                case KEYBOARD:
                    controls = InputDataHelper.KeyboardControls;
                    break;
                case GAMEPAD:
                    controls = InputDataHelper.GamepadControls;
                    break;
            }

            targetBindingData.BindingDisplayInfo.EDITOR_Clear();
            foreach (string s in controls)
            {
                targetBindingData.BindingDisplayInfo.EDITOR_Add(s, new BindingPathInfo());
            }

            EditorUtility.SetDirty(targetBindingData);
            serializedObject.Update();
        }
    }
}