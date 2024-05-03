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
            string[] guids = AssetDatabase.FindAssets($"t:Script {type.Name}");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string dotCsExtensionRemoved = assetPath.Remove(assetPath.Length - 3);
                
                // AssetDatabase.FindAssets exact match operators seem to be broken,
                // so we check for an exact match here instead of in the FindAssets call above.
                if (dotCsExtensionRemoved.EndsWith(type.Name))
                {
                    return Application.dataPath + assetPath.Replace("Assets", string.Empty);
                }
            }

            return string.Empty;
        }
    }
}