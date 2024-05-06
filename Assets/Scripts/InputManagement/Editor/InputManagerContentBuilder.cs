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
                case "DefaultContextEnabler":
                    inputData = EditorAssetGetter.Get<OfflineInputData>();
                    lines.Add($"        private static void EnableDefaultContext() => Enable{inputData.DefaultContext}Context();");
                    break;
                case "ContextEnablers":
                    inputData = EditorAssetGetter.Get<OfflineInputData>();
                    for (int i = 0; i < inputData.InputContexts.Length; i++)
                    {
                        InputContext context = inputData.InputContexts[i];
                        lines.Add($"        public static void Enable{context.Name}Context()");
                        lines.Add("        {");
                        foreach (string mapName in GeneratorHelper.GetCleanedMapNames(asset))
                        {
                            bool enable = context.ActiveMaps.Any(activeMapName => mapName == activeMapName);
                            lines.Add($"            {mapName}.{(enable ? "Enable" : "Disable")}();");
                        }
                        lines.Add(string.Empty);
                        lines.Add("            SetUIEventSystemActions(");
                        string actions = "                ";
                        InputActionReference[] inputActionReferences = context.EventSystemActions.AllInputActionReferences;
                        for (int j = 0; j < inputActionReferences.Length; j++)
                        {
                            if (j > 0 && j % 4 == 0) actions += "\n                ";
                            InputActionReference inputActionReference = inputActionReferences[i];
                            actions += $"{(inputActionReference == null ? "null" : inputActionReference.action.actionMap.name + "." + inputActionReference.action.name)}";
                            if (j < inputActionReferences.Length - 1) actions += ", ";
                        }
                        lines.Add(actions);
                        lines.Add("            );");
                        lines.Add("        }");
                        if (i < inputData.InputContexts.Length - 1)
                        {
                            lines.Add(string.Empty);
                        }
                    }
                    break;
            }
        }
    }
}
