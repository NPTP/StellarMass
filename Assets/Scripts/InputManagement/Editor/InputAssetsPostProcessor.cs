using System.Linq;
using UnityEditor;

namespace StellarMass.InputManagement.Editor
{
    public class InputAssetsPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Any(importedAsset =>
                    importedAsset.StartsWith("Assets/InputConfig") && importedAsset.EndsWith(".inputactions")))
            {
                MapInstanceGenerator.GenerateMapInstances();
            }
        }
    }
}