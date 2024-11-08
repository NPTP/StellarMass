using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StellarMass.Systems.SceneManagement.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        private const string GUID_FIELD_NAME = "guid";
        private const string BUILD_INDEX_FIELD_NAME = "buildIndex";
        
        private static EditorBuildSettingsScene[] GetActiveBuildSettingsScenesWithoutBootstrap()
        {
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length - 1];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (i == 0 || !EditorBuildSettings.scenes[i].enabled) continue;
                scenes[i - 1] = EditorBuildSettings.scenes[i];
            }
            return scenes;
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
            EditorBuildSettingsScene[] scenes = GetActiveBuildSettingsScenesWithoutBootstrap();

            if (scenes.Length <= 1)
            {
                EditorGUI.LabelField(position, $"No scenes found", EditorStyles.helpBox);
                position.y += position.height;
                return;
            }

            SerializedProperty sceneGuidProperty = property.FindPropertyRelative(GUID_FIELD_NAME);
            SerializedProperty buildIndexProperty = property.FindPropertyRelative(BUILD_INDEX_FIELD_NAME);
            
            int index = -1;
            List<string> guids = new List<string>();
            List<string> paths = new List<string>();
            for (int i = 0; i < scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = scenes[i];
                string guid = scene.guid.ToString();
                string path = scene.path.Replace("Assets/Scenes/", string.Empty).Replace(".unity", string.Empty);
                guids.Add(guid);
                paths.Add(path);
                if (guid == sceneGuidProperty.stringValue)
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