using System;
using UnityEditor;
using UnityEngine;

namespace StellarMass.Utilities.Editor
{
    public static class EditorScriptGetter
    {
        public static string GetSystemPath<T>() => GetSystemPath(typeof(T));

        public static string GetSystemPath(Type type)
        {
            bool isEnum = type.IsEnum;
            
            string[] guids = AssetDatabase.FindAssets($"t:Script");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (scriptAsset == null)
                {
                    continue;
                }

                if ((isEnum && scriptAsset.text.Contains($"enum {type.Name}")) ||
                    scriptAsset.GetClass() == type || type.IsAssignableFrom(scriptAsset.GetClass()))
                {
                    return Application.dataPath + assetPath.Replace("Assets", string.Empty);
                }
            }

            return string.Empty;
        }
    }
}