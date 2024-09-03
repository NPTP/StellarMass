using System.Collections.Generic;
using System.Linq;
using StellarMass.Utilities.References;
using UnityEditor;
using UnityEngine;

namespace StellarMass.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        private static EditorBuildSettingsScene[] GetActiveBuildSettingsScenes()
        {
            return EditorBuildSettings.scenes.Where(t => t.enabled).ToArray();
        }

        private static bool TryGetSceneBuildIndex(SceneAsset sceneAsset, out int buildIndex)
        {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                if (scene.path == scenePath)
                {
                    buildIndex = i;
                    return true;
                }
            }

            buildIndex = -1;
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorBuildSettingsScene[] scenes = GetActiveBuildSettingsScenes();

            if (scenes.Length == 0)
            {
                EditorGUI.LabelField(position, $"No scenes found", EditorStyles.helpBox);
                position.y += position.height;
                return;
            }

            SerializedProperty sceneGuidProperty = property.FindPropertyRelative("guid");
            SerializedProperty buildIndexProperty = property.FindPropertyRelative("buildIndex");
            
            int index = -1;
            List<string> guids = new List<string>();
            List<string> paths = new List<string>();
            for (int i = 0; i < scenes.Length; i++)
            {
                guids.Add(scenes[i].guid.ToString());
                paths.Add(scenes[i].path.Replace("Assets/Scenes/", string.Empty).Replace(".unity", string.Empty));
                if (guids[i] == sceneGuidProperty.stringValue)
                {
                    index = i;
                }
            }

            index = Mathf.Clamp(index, 0, scenes.Length - 1);
            Rect dropdownPosition = new Rect(position.x, position.y, position.width, position.height);
            index = EditorGUI.Popup(dropdownPosition, property.displayName, index, paths.ToArray());
            sceneGuidProperty.stringValue = guids[index];
            string fullPath = "Assets/Scenes/" + paths[index] + ".unity";
            SceneAsset chosen = AssetDatabase.LoadAssetAtPath<SceneAsset>(fullPath);
            if (!TryGetSceneBuildIndex(chosen, out int buildIndex))
            {
                return;
            }

            buildIndexProperty.intValue = buildIndex;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}