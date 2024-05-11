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
            if (offlineInputData == null || offlineInputData.RuntimeInputData == null || offlineInputData.RuntimeInputData.InputActionAsset == null)
            {
                return;
            }
            
            if (importedAssets.Any(importedAsset => importedAsset.EndsWith($"{offlineInputData.RuntimeInputData.InputActionAsset.name}.inputactions") ||
                                                    importedAsset.EndsWith($"{offlineInputData.name}.asset")))
            {
                InputScriptGenerator.GenerateMapInstances();
            }
        }
    }
}