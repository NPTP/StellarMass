using System;
using System.Collections.Generic;
using System.IO;
using StellarMass.Editor;
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
        
        private static char S => Path.DirectorySeparatorChar;
        private static string InputManagerPath => Application.dataPath + $@"{S}Scripts{S}InputManagement{S}{nameof(InputManager)}.cs";

        [MenuItem(EditorToolNames.GENERATOR_FEATURE)]
        public static void GenerateMapInstances()
        {
            // InputActionsGenerated inputActions = new();
            string json = File.ReadAllText(Application.dataPath + $"{S}InputConfig{S}InputActions.inputactions");
            InputActionAsset asset = FromJson(json); // inputActions.asset;
            GenerateMapInstanceClasses(asset);
            UpdateActionMapEnum(asset);
            ModifyInputManager(asset);
            
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static void UpdateActionMapEnum(InputActionAsset asset)
        {
            List<string> newLines = new();

            try
            {
                using StreamReader sr = new(GeneratorHelper.GetEnumFilePath());
                ReadState readState = ReadState.Normal;
                while (sr.ReadLine() is { } line)
                {
                    switch (readState)
                    {
                        case ReadState.Normal:
                            if (GeneratorHelper.IsMarkerStart(line, out string markerName))
                            {
                                ActionMapEnumContentBuilder.AddContentForActionMapEnum(asset, markerName, newLines);
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
            }

            GeneratorHelper.WriteLinesToFile(newLines, GeneratorHelper.GetEnumFilePath());
        }


        private static InputActionAsset FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
            asset.LoadFromJson(json);
            return asset;
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
            }

            GeneratorHelper.WriteLinesToFile(newLines, GeneratorHelper.GetPathForGeneratedMap(map.name));
        }
        
        private static void ModifyInputManager(InputActionAsset asset)
        {
            List<string> newLines = new();

            try
            {
                using StreamReader sr = new(InputManagerPath);
                ReadState readState = ReadState.Normal;
                while (sr.ReadLine() is { } line)
                {
                    switch (readState)
                    {
                        case ReadState.Normal:
                            newLines.Add(line);
                            if (GeneratorHelper.IsMarkerStart(line, out string markerName))
                            {
                                InputManagerContentBuilder.AddContentForInputManager(asset, markerName, newLines);
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
            }

            GeneratorHelper.WriteLinesToFile(newLines, InputManagerPath);
        }
    }
}