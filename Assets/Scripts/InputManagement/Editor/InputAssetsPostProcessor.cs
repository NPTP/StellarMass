using System.Linq;
using StellarMass.Data;
using UnityEditor;

namespace StellarMass.InputManagement.Editor
{
    public class InputAssetsPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Any(importedAsset => importedAsset.EndsWith(".inputactions") ||
                                                    importedAsset.EndsWith($"{nameof(OfflineInputData)}.asset")))
            {
                InputManagerScriptGenerator.GenerateMapInstances();
            }
        }
    }
}