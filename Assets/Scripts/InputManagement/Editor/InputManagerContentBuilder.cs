using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class InputManagerContentBuilder
    {
        public static void AddContentForInputManager(InputActionAsset asset, string markerName, List<string> newLines)
        {
            switch (markerName)
            {
                case "MapInstanceProperties":
                    newLines.AddRange(asset.actionMaps.Select(map =>
                        $"        public static {map.name} {map.name}" + " { get; private set; }"));
                    break;
                case "InstantiateMapInstances":
                    newLines.AddRange(asset.actionMaps.Select(map =>
                        $"            {map.name} = new {map.name}(inputActions.{map.name});"));
                    break;
                case "CollectionInitializer":
                    newLines.AddRange(asset.actionMaps.Select(map =>
                        $"                {map.name},"));
                    break;
            }
        }
    }
}