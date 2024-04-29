using System;
using System.Collections.Generic;
using System.IO;
using StellarMass.Editor;
using StellarMass.InputManagement.MapInstances;
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
        private static string MapInstanceTemplateName => nameof(MapInstanceTemplate);

        // NP TODO: Find a way to call the generator after the input asset is saved.
        // Had issues using AssetPostProcessor where the input asset classes this draws from weren't ready yet, ie this got called too early.
        [MenuItem(EditorToolNames.GENERATOR_FEATURE)]
        public static void GenerateMapInstances()
        {
            InputActionsGenerated inputActions = new();
            InputActionAsset asset = inputActions.asset;
            foreach (InputActionMap map in asset.actionMaps)
            {
                GenerateMapInstanceClasses(map);
            }

            ModifyInputManager(asset);
            
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateMapInstanceClasses(InputActionMap map)
        {
            List<string> newLines = new();

            try
            {
                using StreamReader sr = new(GeneratorHelper.GetFilePathForMapName(MapInstanceTemplateName));
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

            GeneratorHelper.WriteLinesToFile(newLines, GeneratorHelper.GetFilePathForMapName(map.name));
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