using System;
using System.Collections.Generic;
using System.IO;
using StellarMass.Editor;
using StellarMass.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class InputManagerScriptGenerator
    {
        private enum ReadState
        {
            Normal = 0,
            WaitingForMarkerEnd
        }

        private static string InputManagerFilePath => EditorScriptGetter.GetSystemPath(typeof(InputManager));
        private static string ControlSchemeFilePath => EditorScriptGetter.GetSystemPath<ControlScheme>();

        [MenuItem(EditorToolNames.GENERATOR_FEATURE)]
        public static void GenerateMapInstances()
        {
            InputActionAsset asset = GeneratorHelper.InputActionAsset;
            GenerateMapInstanceClasses(asset);
            ModifyExistingFile(asset, InputManagerFilePath, InputManagerContentBuilder.AddContentForInputManager);
            ModifyExistingFile(asset, ControlSchemeFilePath, ControlSchemeContentBuilder.AddContentForControlScheme);
            
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateMapInstanceClasses(InputActionAsset asset)
        {
            GeneratorHelper.ClearGeneratedFolder();
            
            foreach (InputActionMap map in asset.actionMaps)
            {
                GenerateMapInstanceClass(map);
            }
        }

        private static void GenerateMapInstanceClass(InputActionMap map)
        {
            List<string> newLines = new();

            try
            {
                using StreamReader sr = new(GeneratorHelper.GetTemplateFilePath());
                ReadState readState = ReadState.Normal;
                while (sr.ReadLine() is { } line)
                {
                    switch (readState)
                    {
                        case ReadState.Normal:
                            if (GeneratorHelper.IsMarkerStart(line, out string markerName))
                            {
                                MapInstanceContentBuilder.AddContentForMapInstance(markerName, map, newLines);
                                readState = ReadState.WaitingForMarkerEnd;
                            }
                            else
                            {
                                newLines.Add(line);
                            }
                            break;
                        case ReadState.WaitingForMarkerEnd:
                            if (GeneratorHelper.IsMarkerEnd(line)) readState = ReadState.Normal;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"The file could not be read: {e.Message}");
                return;
            }

            GeneratorHelper.WriteLinesToFile(newLines, GeneratorHelper.GetPathForGeneratedMap(map.name));
        }
        
        private static void ModifyExistingFile(InputActionAsset asset, string filePath, Action<InputActionAsset, string, List<string>> markerSectionAction)
        {
            List<string> newLines = new();

            try
            {
                using StreamReader sr = new(filePath);
                ReadState readState = ReadState.Normal;
                while (sr.ReadLine() is { } line)
                {
                    switch (readState)
                    {
                        case ReadState.Normal:
                            newLines.Add(line);
                            if (GeneratorHelper.IsMarkerStart(line, out string markerName))
                            {
                                markerSectionAction?.Invoke(asset, markerName, newLines);
                                readState = ReadState.WaitingForMarkerEnd;
                            }
                            break;
                        case ReadState.WaitingForMarkerEnd:
                            if (GeneratorHelper.IsMarkerEnd(line))
                            {
                                newLines.Add(line);
                                readState = ReadState.Normal;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"The file could not be read: {e.Message}");
                return;
            }

            GeneratorHelper.WriteLinesToFile(newLines, filePath);
        }
    }
}