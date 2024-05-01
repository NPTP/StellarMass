using System.Collections.Generic;
using StellarMass.Utilities.Extensions;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class ActionMapEnumContentBuilder
    {
        public static void AddContentForActionMapEnum(InputActionAsset asset, string markerName, List<string> lines)
        {
            if (markerName == "EnumMembers")
            {
                foreach (InputActionMap map in asset.actionMaps)
                {
                    lines.Add($"        {map.name.AllWhitespaceTrimmed()},");
                }
            }
        }
    }
}