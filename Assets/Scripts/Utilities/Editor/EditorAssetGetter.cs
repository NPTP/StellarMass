using UnityEditor;
using UnityEngine;

namespace StellarMass.Utilities.Editor
{
    public static class EditorAssetGetter
    {
        public static T GetFirst<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length == 0) return null;
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}