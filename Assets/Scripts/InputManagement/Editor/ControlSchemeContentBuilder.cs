using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class ControlSchemeContentBuilder
    {
        public static void AddContentForControlScheme(InputActionAsset asset, string markerName, List<string> lines)
        {
            switch (markerName)
            {
                case "Members":
                    foreach (InputControlScheme inputControlScheme in asset.controlSchemes)
                        lines.Add($"        {inputControlScheme.name},");
                    break;
            }
        }
    }
}