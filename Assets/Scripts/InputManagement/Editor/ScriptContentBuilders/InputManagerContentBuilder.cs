using System.Collections.Generic;
using System.Linq;
using StellarMass.InputManagement.Data;
using StellarMass.Utilities.Editor;
using StellarMass.Utilities.Extensions;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor.ScriptContentBuilders
{
    public static class InputManagerContentBuilder
    {
        public static void AddContentForInputManager(InputActionAsset asset, string markerName, List<string> lines)
        {
            OfflineInputData inputData;
            string inputActionsField = "inputActions";

            switch (markerName)
            {
                case "RuntimeInputDataAddress":
                    lines.Add($"        private const string RUNTIME_INPUT_DATA_ADDRESS = \"{OfflineInputData.RUNTIME_INPUT_DATA_ADDRESS}\";");
                    break;
                case "UsingDirective":
                    string inputActionsNamespace = GeneratorHelper.IInputActionsNamespace;
                    if (!string.IsNullOrEmpty(inputActionsNamespace))
                        lines.Add($"using {inputActionsNamespace};");
                    break;
                case "MapActionsProperties":
                    foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        lines.Add($"        public static {mapName}Actions {mapName}" + " { get; private set; }");
                    break;
                case "InputActionCollectionDeclaration":
                    lines.Add($"        private static {GeneratorHelper.IInputActionsClassName} {inputActionsField};");
                    break;                
                case "DefaultContextProperty":
                    inputData = EditorAssetGetter.GetFirst<OfflineInputData>();
                    lines.Add($"        private static {nameof(InputContext)} DefaultContext => {nameof(InputContext)}.{inputData.DefaultContext};");
                    break;
                case "InputActionCollectionInstantiation":
                    lines.Add($"            {inputActionsField} = new {GeneratorHelper.IInputActionsClassName}();");
                    break;
                case "InstantiateMapInstances":
                    foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        lines.Add($"            {mapName} = new {mapName}Actions();");
                    break;
                case "MapActionsRemoveCallbacks":
                    foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        lines.Add($"            {inputActionsField}.{mapName}.RemoveCallbacks({mapName});");
                    break;
                case "ControlSchemeSwitch":
                    foreach (InputControlScheme controlScheme in asset.controlSchemes)
                        lines.Add($"                \"{controlScheme.name}\" => {nameof(ControlScheme)}.{controlScheme.name.AlphaNumericCharactersOnly()},");
                    break;
                case "EnableContextSwitchMembers":
                    inputData = EditorAssetGetter.GetFirst<OfflineInputData>();
                    foreach (InputContextInfo contextInfo in inputData.InputContextInfos)
                    {
                        lines.Add($"                case {nameof(InputContext)}.{contextInfo.Name}:");
                        foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        {
                            bool enable = contextInfo.ActiveMaps.Any(activeMapName => mapName == activeMapName);
                            lines.Add($"                    {inputActionsField}.{mapName}.{(enable ? "Enable" : "Disable")}();");
                            lines.Add($"                    {inputActionsField}.{mapName}.{(enable ? "Add" : "Remove")}Callbacks({mapName});");
                        }

                        if (contextInfo.UseEventSystemActions)
                        {
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
                        }

                        lines.Add($"                    break;");
                    }
                    break;
            }
        }
    }
}
