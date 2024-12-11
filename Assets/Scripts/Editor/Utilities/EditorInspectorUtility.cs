using UnityEditor;
using UnityEngine;

namespace Summoner.Editor.Utilities
{
    public static class EditorInspectorUtility
    {
        public static void DrawHorizontalLine()
        {
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }
        
        internal static void ShowScriptInspector<T>(T targetMonoBehaviour) where T : MonoBehaviour
        {
            EditorGUI.BeginDisabledGroup(disabled: true);
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(targetMonoBehaviour), typeof(T), false);
            EditorGUI.EndDisabledGroup();
        }
        
        public static T GetTargetObject<T>(SerializedProperty property) where T : class
        {
            if (property == null)
                return null;

            object obj = property.serializedObject.targetObject;
            string[] fieldHierarchy = property.propertyPath.Split('.');

            foreach (string fieldName in fieldHierarchy)
            {
                if (obj == null) return null;

                // Handle array elements
                if (fieldName.StartsWith("data["))
                {
                    // Extract the index from the field name
                    int startIndex = fieldName.IndexOf('[') + 1;
                    int endIndex = fieldName.IndexOf(']');
                    int arrayIndex = int.Parse(fieldName.Substring(startIndex, endIndex - startIndex));

                    // Get the array and access the specific element
                    if (obj is System.Collections.IList list && arrayIndex >= 0 && arrayIndex < list.Count)
                    {
                        obj = list[arrayIndex];
                    }
                }
                else
                {
                    var type = obj.GetType();
                    var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (field == null) continue;

                    obj = field.GetValue(obj);
                }
            }

            return obj as T;
        }
    }
}