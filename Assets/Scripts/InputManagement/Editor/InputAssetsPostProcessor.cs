using System.Linq;
using StellarMass.InputManagement.Data;
using StellarMass.Utilities.Editor;
using UnityEditor;

namespace StellarMass.InputManagement.Editor
{
    public class InputAssetsPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            OfflineInputData offlineInputData = EditorAssetGetter.GetFirst<OfflineInputData>();
            if (importedAssets.Any(importedAsset => importedAsset.EndsWith($"{offlineInputData.InputActionAsset.name}.inputactions") ||
                                                    importedAsset.EndsWith($"{offlineInputData.name}.asset")))
            {
                InputScriptGenerator.GenerateMapInstances();
            }
        }
    }
}