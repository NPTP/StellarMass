using UnityEditor;
using Object = UnityEngine.Object;

namespace StellarMass.Utilities.Extensions
{
    public static class ObjectExtensions
    {
        public static string AssetGUID(this Object obj)
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
        }
    }
}