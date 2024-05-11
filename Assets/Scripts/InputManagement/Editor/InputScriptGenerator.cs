using System;
using System.Collections.Generic;
using System.IO;
using StellarMass.Editor;
using StellarMass.InputManagement.Data;
using StellarMass.InputManagement.Editor.ScriptContentBuilders;
using StellarMass.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class InputScriptGenerator
    {
        private enum ReadState
        {
            Normal = 0,
            WaitingForMarkerEnd
        }

        private static string InputManagerFilePath => EditorScriptGetter.GetSystemPath(typeof(Input));
        private static string ControlSchemeFilePath => EditorScriptGetter.GetSystemPath<ControlScheme>();
        private static string InputContextFilePath => EditorScriptGetter.GetSystemPath<InputContext>();
        private static InputActionAsset InputActionAsset => EditorAssetGetter.GetFirst<RuntimeInputData>().InputActionAsset;
        private static char S => Path.DirectorySeparatorChar;
        private static string GeneratedFolderPath => $@"{S}Scripts{S}InputManagement{S}Generated{S}";
        private static string GeneratedMapActionsPath => $@"{S}Scripts{S}InputManagement{S}Generated{S}MapActions{S}";
        private static string GeneratedMapCachePath => $@"{S}Scripts{S}InputManagement{S}Generated{S}MapCaches{S}";

        [MenuItem(EditorToolNames.GENERATOR_FEATURE)]
        public static void GenerateMapInstances()
        {
            InputActionAsset asset = InputActionAsset;
            GeneratorHelper.ClearFolder(GeneratedFolderPath);
            GenerateMapActionClasses(asset);
            GenerateMapCacheClasses(asset);
            ModifyExistingFile(asset, ControlSchemeFilePath, ControlSchemeContentBuilder.AddContent);
            ModifyExistingFile(asset, InputContextFilePath, InputContextContentBuilder.AddContent);
            ModifyExistingFile(asset, InputManagerFilePath, InputManagerContentBuilder.AddContent);
            
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateMapActionClasses(InputActionAsset asset)
        {
            foreach (InputActionMap map in asset.actionMaps)
            {
                GenerateFile(map, 
                    GeneratorHelper.GetMapActionsTemplateFilePath(), 
                    MapActionsContentBuilder.AddContent,
                    GeneratorHelper.GetPathForGeneratedClass(GeneratedMapActionsPath + map.name.AsType() + "Actions.cs"));
            }
        }
        
        private static void GenerateMapCacheClasses(InputActionAsset asset)
        {
            foreach (InputActionMap map in asset.actionMaps)
            {
                GenerateFile(map, 
                    GeneratorHelper.GetMapCacheTemplateFilePath(), 
                    MapCachesContentBuilder.AddContent,
                    GeneratorHelper.GetPathForGeneratedClass(GeneratedMapCachePath + map.name.AsType() + "MapCache.cs"));
            }
        }

        private static void GenerateFile(InputActionMap map, string readPath,
            Action<string, InputActionMap, List<string>> addContentAction, string writePath)
        {
            List<string> newLines = new();

            try
            {
                using StreamReader sr = new(readPath);
                ReadState readState = ReadState.Normal;
                while (sr.ReadLine() is { } line)
                {
                    switch (readState)
                    {
                        case ReadState.Normal:
                            if (GeneratorHelper.IsMarkerStart(line, out string markerName))
                            {
                                addContentAction(markerName, map, newLines);
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

            GeneratorHelper.WriteLinesToFile(newLines, writePath);
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