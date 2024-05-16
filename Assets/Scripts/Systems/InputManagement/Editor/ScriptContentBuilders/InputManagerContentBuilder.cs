using System.Collections.Generic;
using StellarMass.Systems.InputManagement.Data;
using UnityEngine.InputSystem;

namespace StellarMass.Systems.InputManagement.Editor.ScriptContentBuilders
{
    public static class InputManagerContentBuilder
    {
        public static void AddContent(InputActionAsset asset, string markerName, List<string> lines)
        {
            switch (markerName)
            {
                case "RuntimeInputAddress":
                    lines.Add($"        private const string RUNTIME_INPUT_DATA_ADDRESS = \"{OfflineInputData.RUNTIME_INPUT_DATA_ADDRESS}\";");
                    break;
                case "PublicPlayerGetter":
                    if (!Helper.OfflineInputData.SinglePlayerOnly)
                        lines.Add($"        public static bool TryGetPlayer(int id, out {nameof(InputPlayer)} inputPlayer) => playerCollection.TryGetPlayer(id, out inputPlayer);");
                    break;
                case "SinglePlayerPublicProperties":
                    if (!Helper.OfflineInputData.SinglePlayerOnly)
                        break;
                    foreach (string mapName in Helper.GetMapNames(asset))
                        lines.Add($"        public static {mapName.AsType()}Actions {mapName.AsType()} => primaryPlayer.{mapName.AsType()};");
                    lines.Add($"        public static {nameof(InputContext)} CurrentContext => primaryPlayer.CurrentContext;");
                    lines.Add($"        public static {nameof(ControlScheme)} CurrentControlScheme => primaryPlayer.CurrentControlScheme;");
                    lines.Add($"        public static void EnableContext({nameof(InputContext)} context) => primaryPlayer.EnableContext(context);");
                    break;
                case "DefaultContextProperty":
                    lines.Add($"        private static {nameof(InputContext)} DefaultContext => {nameof(InputContext)}.{Helper.OfflineInputData.DefaultContext};");
                    break;
            }
        }
    }
}
