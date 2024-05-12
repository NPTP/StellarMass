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
                case "PlayerGetter":
                    string privateOrPublic = Helper.OfflineInputData.SinglePlayerOnly ? "private" : "public";
                    lines.Add($"        {privateOrPublic} static {nameof(InputPlayer)} Player(int id) => playerCollection.GetPlayer(id);");
                    break;
                case "SinglePlayerProperties":
                    if (!Helper.OfflineInputData.SinglePlayerOnly)
                        break;
                    foreach (string mapName in Helper.GetMapNames(asset))
                        lines.Add($"        public static {mapName.AsType()}Actions {mapName.AsType()} => Player(0).{mapName.AsType()};");
                    lines.Add($"        public static {nameof(InputContext)} CurrentContext => Player(0).CurrentContext;");
                    lines.Add($"        public static {nameof(ControlScheme)} CurrentControlScheme => Player(0).CurrentControlScheme;");
                    lines.Add($"        public static void EnableContext({nameof(InputContext)} context) => Player(0).EnableContext(context);");
                    break;
                case "DefaultContextProperty":
                    lines.Add($"        private static {nameof(InputContext)} DefaultContext => {nameof(InputContext)}.{Helper.OfflineInputData.DefaultContext};");
                    break;
            }
        }
    }
}
