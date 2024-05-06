using System.Collections.Generic;
using StellarMass.InputManagement.Data;
using StellarMass.Utilities.Editor;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class InputContextContentBuilder
    {
        public static void AddContentForInputContext(InputActionAsset asset, string markerName, List<string> lines)
        {
            switch (markerName)
            {
                case "Members":
                    OfflineInputData offlineInputData = EditorAssetGetter.Get<OfflineInputData>();
                    foreach (InputContextInfo contextInfo in offlineInputData.InputContextInfos)
                        lines.Add($"        {contextInfo.Name},");
                    break;
            }
        }
    }
}