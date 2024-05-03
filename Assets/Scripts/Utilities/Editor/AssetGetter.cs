using UnityEditor;
using UnityEngine;

namespace StellarMass.Utilities.Editor
{
    public static class AssetGetter
    {
        public static T GetAsset<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length == 0) return null;
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}