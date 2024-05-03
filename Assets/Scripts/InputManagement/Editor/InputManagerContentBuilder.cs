using System.Collections.Generic;
using System.Linq;
using StellarMass.Data;
using StellarMass.Utilities.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class InputManagerContentBuilder
    {
        public static void AddContentForInputManager(InputActionAsset asset, string markerName, List<string> lines)
        {
            OfflineInputData inputData = null;

            switch (markerName)
            {
                case "MapInstanceProperties":
                    foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        lines.Add($"        public static {mapName} {mapName}" + " { get; private set; }");
                    break;
                case "InstantiateMapInstances":
                    foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        lines.Add($"            {mapName} = new {mapName}(inputActions.{mapName});");
                    break;
                case "CollectionInitializer":
                    foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        lines.Add($"                {mapName},");
                    break;
                case "ControlSchemeSwitch":
                    foreach (InputControlScheme controlScheme in asset.controlSchemes)
                    {
                        lines.Add($"                \"{controlScheme.name}\" => {nameof(ControlScheme)}.{controlScheme.name},");
                    }
                    break;
                case "DefaultContextEnabler":
                    inputData = EditorAssetGetter.Get<OfflineInputData>();
                    lines.Add($"        private static void EnableDefaultContext() => Enable{inputData.DefaultContext}Context();");
                    break;
                case "ContextEnablers":
                    inputData = EditorAssetGetter.Get<OfflineInputData>();
                    for (int i = 0; i < inputData.InputContexts.Length; i++)
                    {
                        InputContext context = inputData.InputContexts[i];
                        lines.Add($"        public static void Enable{context.Name}Context()");
                        lines.Add("        {");
                        foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        {
                            bool enable = context.ActiveMaps.Any(activeMapName => mapName == activeMapName);
                            lines.Add($"            {mapName}.{(enable ? "Enable" : "Disable")}();");
                        }
                        lines.Add("        }");
                        if (i < inputData.InputContexts.Length - 1)
                        {
                            lines.Add(string.Empty);
                        }
                    }
                    break;
            }
        }
    }
}
