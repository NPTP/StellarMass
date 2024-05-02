using System.Collections.Generic;
using System.Linq;
using StellarMass.Data;
using StellarMass.Utilities.Extensions;
using UnityEditor;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class InputManagerContentBuilder
    {
        public static void AddContentForInputManager(InputActionAsset asset, string markerName, List<string> lines)
        {
            foreach (string mapName in asset.actionMaps.Select(map => map.name.AllWhitespaceTrimmed().CapitalizeFirst()))
            {
                string line = null;
                
                switch (markerName)
                {
                    case "MapInstanceProperties":
                        line = $"        public static {mapName} {mapName}" + " { get; private set; }";
                        break;
                    case "InstantiateMapInstances":
                        line = $"            {mapName} = new {mapName}(inputActions.{mapName});";
                        break;
                    case "CollectionInitializer":
                        line = $"                {mapName},";
                        break;
                }

                if (line != null)
                {
                    lines.Add(line);
                }
            }

            if (markerName == "ContextEnablers")
            {
                OfflineInputData inputData = AssetDatabase.LoadAssetAtPath<OfflineInputData>("Assets/ScriptableObjects/OfflineData/OfflineInputData.asset");

                for (int i = 0; i < inputData.ActionContexts.Length; i++)
                {
                    InputContext context = inputData.ActionContexts[i];
                    
                    string methodLine = $"        public static void Enable{context.Name}Context()\n";
                    methodLine += "        {\n";
                    foreach (string mapName in asset.actionMaps.Select(map =>
                                 map.name.AllWhitespaceTrimmed().CapitalizeFirst()))
                    {
                        bool enable = context.ActiveMaps.Any(activeMapName => mapName == activeMapName);
                        methodLine += $"            {mapName}.{(enable ? "Enable" : "Disable")}();\n";
                    }

                    methodLine += "        }";
                    if (i < inputData.ActionContexts.Length - 1)
                    {
                        methodLine += "\n";
                    }
                    
                    lines.Add(methodLine);
                }
            }
        }
    }
}