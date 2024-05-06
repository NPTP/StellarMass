using System.Collections.Generic;
using System.Linq;
using StellarMass.InputManagement.Data;
using StellarMass.Utilities.Editor;
using UnityEditor;
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
                case "RuntimeInputDataPath":
                    string path = AssetDatabase.GetAssetPath(EditorAssetGetter.Get<OfflineInputData>().RuntimeInputData);
                    lines.Add($"        private const string RUNTIME_INPUT_DATA_PATH = \"{path}\";");
                    break;
                case "UsingDirective":
                    lines.Add($"using {GeneratorHelper.IInputActionsNamespace};");
                    break;
                case "MapInstanceProperties":
                    foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        lines.Add($"        public static {mapName} {mapName}" + " { get; private set; }");
                    break;
                case "InputActionCollectionDeclaration":
                    lines.Add($"        private static {GeneratorHelper.IInputActionsClassName} inputActions;");
                    break;                
                case "DefaultContextProperty":
                    inputData = EditorAssetGetter.Get<OfflineInputData>();
                    lines.Add($"        private static {nameof(InputContext)} DefaultContext => {nameof(InputContext)}.{inputData.DefaultContext};");
                    break;
                case "InputActionCollectionInstantiation":
                    lines.Add($"            inputActions = new {GeneratorHelper.IInputActionsClassName}();");
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
                        lines.Add($"                \"{controlScheme.name}\" => {nameof(ControlScheme)}.{controlScheme.name},");
                    break;
                case "EnableContextSwitchMembers":
                    inputData = EditorAssetGetter.Get<OfflineInputData>();
                    foreach (InputContextInfo contextInfo in inputData.InputContextInfos)
                    {
                        lines.Add($"                case {nameof(InputContext)}.{contextInfo.Name}:");
                        foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        {
                            bool enable = contextInfo.ActiveMaps.Any(activeMapName => mapName == activeMapName);
                            lines.Add($"                    {mapName}.{(enable ? "Enable" : "Disable")}();");
                        }
                        string actions = "                    SetUIEventSystemActions(";
                        InputActionReference[] inputActionReferences = contextInfo.EventSystemActions.AllInputActionReferences;
                        for (int j = 0; j < inputActionReferences.Length; j++)
                        {
                            InputActionReference inputActionReference = inputActionReferences[j];
                            actions += $"{(inputActionReference == null ? "null" : inputActionReference.action.actionMap.name + "." + inputActionReference.action.name)}";
                            if (j < inputActionReferences.Length - 1) actions += ", ";
                        }
                        actions += ");";
                        lines.Add(actions);
                        lines.Add($"                    break;");
                    }
                    break;
            }
        }
    }
}
