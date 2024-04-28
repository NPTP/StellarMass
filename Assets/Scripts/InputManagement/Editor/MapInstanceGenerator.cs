using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StellarMass.Editor;
using StellarMass.InputManagement.MapInstances;
using StellarMass.Utilities.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class MapInstanceGenerator
    {
        private enum ReadState
        {
            Normal = 0,
            WaitingForMarkerEnd
        }
        
        private const string MAP_INSTANCE_TEMPLATE_NAME = "MapInstanceTemplate";
        private const string MARKER = "// MARKER";
        private const string START = "Start";
        private const string END = "End";
        
        private static char S => Path.DirectorySeparatorChar;
        private static string MapInstancesPath => $@"{S}Scripts{S}InputManagement{S}MapInstances{S}";

        [MenuItem(EditorToolNames.GENERATOR_FEATURE)]
        public static void GenerateMapInstances()
        {
            InputActionsGenerated inputActions = new();
            InputActionAsset asset = inputActions.asset;
            foreach (InputActionMap map in asset.actionMaps)
            {
                WriteToFile(map);
            }
        }

        private static void WriteToFile(InputActionMap map)
        {
            List<string> newLines = new();

            try
            {
                using StreamReader sr = new(GetFilePathForMapName(MAP_INSTANCE_TEMPLATE_NAME));
                ReadState readState = ReadState.Normal;
                while (sr.ReadLine() is { } line)
                {
                    switch (readState)
                    {
                        case ReadState.Normal:
                            if (IsMarkerStart(line, out string markerName))
                            {
                                AddContent(markerName, map, newLines);
                                readState = ReadState.WaitingForMarkerEnd;
                            }
                            else
                            {
                                newLines.Add(line);
                            }
                            break;
                        case ReadState.WaitingForMarkerEnd:
                            if (IsMarkerEnd(line)) readState = ReadState.Normal;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"The file could not be read: {e.Message}");
            }

            string filePathToWrite = GetFilePathForMapName(map.name);

            try
            {
                using (StreamWriter sw = new(filePathToWrite))
                {
                    foreach (string line in newLines)
                    {
                        sw.WriteLine(line);
                    }
                }

                Debug.Log($"File {filePathToWrite} written successfully!");
                
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            catch (Exception e)
            {
                Debug.Log($"The file could not be written: {e.Message}");
            }
        }
        
        private static string GetFilePathForMapName(string mapName)
        {
            return Application.dataPath + MapInstancesPath + mapName + ".cs";
        }

        private static bool IsMarkerStart(string line, out string markerName)
        {
            string trimmedLine = line.Trim();
            bool isMarkerStart = trimmedLine.StartsWith(MARKER) && trimmedLine.EndsWith(START);

            if (!isMarkerStart)
            {
                markerName = string.Empty;
                return false;
            }
            
            StringBuilder sb = new();
            int periodCount = 0;
            foreach (char c in trimmedLine)
            {
                if (c == '.')
                {
                    periodCount++;
                    if (periodCount == 2)
                    {
                        break;
                    }
                    continue;
                }

                if (periodCount == 1)
                {
                    sb.Append(c);
                }
            }

            markerName = sb.ToString();
            return true;
        }

        private static bool IsMarkerEnd(string line)
        {
            string trimmedLine = line.Trim();
            return trimmedLine.StartsWith(MARKER) && trimmedLine.EndsWith(END);
        }

        private static void AddContent(string markerName, InputActionMap map, List<string> lines)
        {
            string mapName = map.name;
            string interfaceName = $"I{mapName}Actions";
            string inputActionsGeneratedName = nameof(InputActionsGenerated);
            string mapInstanceName = nameof(MapInstance);
            
            switch (markerName)
            {
                case "Ignore":
                    break;
                case "ClassSignature":
                    classSignature();
                    break;
                case "ActionsGetterProperty":
                    actionsGetterProperty();
                    break;
                case "PublicEvents":
                    publicEvents();
                    break;
                case "ConstructorSignature":
                    constructorSignature();
                    break;
                case "SetUpActions":
                    setUpActions();
                    break;
                case "AddCallbacks":
                    addCallbacks();
                    break;
                case "RemoveCallbacks":
                    removeCallbacks();
                    break;
                case "InterfaceMethods":
                    interfaceMethods();
                    break;
            }
            
            void classSignature()
            {
                lines.Add("    ///<summary>");
                lines.Add($"    /// This class was automatically generated by {nameof(MapInstanceGenerator)} at {DateTime.Now}.");
                lines.Add("    ///</summary>");
                lines.Add("    [Serializable]");
                lines.Add($"    public class {mapName} : MapInstance, InputActionsGenerated.{interfaceName}");
            }

            void actionsGetterProperty()
            {
                lines.Add($"        private {inputActionsGeneratedName}.{mapName}Actions {mapName}Actions" + " { get; }");
            }

            void publicEvents()
            {
                foreach (InputAction inputAction in map.actions)
                {
                    string trimmedName = inputAction.name.AllWhitespaceTrimmed();
                    List<string> arguments = new() { nameof(InputActionPhase) };
                    if (inputAction.expectedControlType != "Button")
                    {
                        arguments.Add(ControlTypeTranslator.Translate(inputAction.expectedControlType));
                    }
                    StringBuilder argumentsString = new();
                    for (int i = 0; i < arguments.Count; i++)
                    {
                        string s = arguments[i];
                        argumentsString.Append(s);
                        if (i < arguments.Count - 1)
                        {
                            argumentsString.Append(", ");
                        }
                    }

                    lines.Add($"        public event Action<{argumentsString}> @On{trimmedName};");
                }
            }

            void constructorSignature()
            {
                lines.Add($"        public {mapName}({inputActionsGeneratedName}.{mapName}Actions actions)");
            }

            void setUpActions()
            {
                lines.Add($"            {mapName}Actions = actions;");
                lines.Add($"            ActionMap = {mapName}Actions.Get();");
            }

            void addCallbacks()
            {
                lines.Add($"        protected sealed override void AddCallbacks() => {mapName}Actions.AddCallbacks(this);");
            }

            void removeCallbacks()
            {
                lines.Add($"        protected sealed override void RemoveCallbacks() => {mapName}Actions.RemoveCallbacks(this);");
            }

            void interfaceMethods()
            {
                foreach (InputAction inputAction in map.actions)
                {
                    string trimmedName = inputAction.name.AllWhitespaceTrimmed();
                    List<string> arguments = new() { "context.phase" };
                    if (inputAction.expectedControlType != "Button")
                    {
                        arguments.Add($"context.ReadValue<{ControlTypeTranslator.Translate(inputAction.expectedControlType)}>()");
                    }

                    StringBuilder argumentsString = new();
                    for (int i = 0; i < arguments.Count; i++)
                    {
                        string s = arguments[i];
                        argumentsString.Append(s);
                        if (i < arguments.Count - 1)
                        {
                            argumentsString.Append(", ");
                        }
                    }

                    lines.Add($"        void {inputActionsGeneratedName}.{interfaceName}.On{trimmedName}(InputAction.CallbackContext context) => On{trimmedName}?.Invoke({argumentsString});");
                }
            }
        }
    }
}