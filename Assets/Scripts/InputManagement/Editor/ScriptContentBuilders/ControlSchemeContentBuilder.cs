using System.Collections.Generic;
using StellarMass.Utilities.Extensions;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor.ScriptContentBuilders
{
    public static class ControlSchemeContentBuilder
    {
        public static void AddContentForControlScheme(InputActionAsset asset, string markerName, List<string> lines)
        {
            switch (markerName)
            {
                case "Members":
                    foreach (InputControlScheme inputControlScheme in asset.controlSchemes)
                        lines.Add($"        {inputControlScheme.name.AlphaNumericCharactersOnly()},");
                    break;
            }
        }
    }
}