using System.Collections.Generic;
using System.Linq;
using StellarMass.Utilities.Extensions;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class InputManagerContentBuilder
    {
        public static void AddContentForInputManager(InputActionAsset asset, string markerName, List<string> lines)
        {
            foreach (string mapName in asset.actionMaps.Select(map => map.name.AllWhitespaceTrimmed()))
            {
                switch (markerName)
                {
                    case "MapInstanceProperties":
                        lines.Add($"        public static {mapName} {mapName}" + " { get; private set; }");
                        break;
                    case "InstantiateMapInstances":
                        lines.Add($"            {mapName} = new {mapName}(inputActions.{mapName});");
                        break;
                    case "CollectionInitializer":
                        lines.Add($"                {mapName},");
                        break;
                }
            }
        }
    }
}