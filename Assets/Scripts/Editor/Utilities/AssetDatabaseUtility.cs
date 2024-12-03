using UnityEditor;

namespace Summoner.Editor.Utilities
{
    public static class AssetDatabaseUtility
    {
        public static bool TryLoadAsset<T>(out T asset) where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset != null;
        }
    }
}