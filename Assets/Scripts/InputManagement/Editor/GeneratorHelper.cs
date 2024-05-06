using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using StellarMass.InputManagement.Data;
using StellarMass.Utilities.Editor;
using StellarMass.Utilities.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.Editor
{
    public static class GeneratorHelper
    {
        private const string MARKER = "// MARKER";
        private const string START = "Start";
        private const string END = "End";
        
        public static InputActionAsset InputActionAsset => EditorAssetGetter.Get<OfflineInputData>().InputActionAsset;
        private static char S => Path.DirectorySeparatorChar;
        private static string MapActionsTemplatePath => $@"{S}Scripts{S}InputManagement{S}MapInstances{S}MapActionsTemplate.txt";
        private static string GeneratedMapsPath => $@"{S}Scripts{S}InputManagement{S}MapInstances{S}Generated{S}";
        public static string IInputActionsNamespace => GetInputActionImporterStringFieldValue("m_WrapperCodeNamespace");
        public static string IInputActionsClassName => GetInputActionImporterStringFieldValue("m_WrapperClassName");

        private static string GetInputActionImporterStringFieldValue(string fieldName)
        {
            const string assemblyName = "Unity.InputSystem";
            const string namespaceName = "UnityEngine.InputSystem.Editor";
            const string typeName = "InputActionImporter";
            
            Assembly assembly = Assembly.Load(assemblyName);
            Type myClassType = assembly.GetType($"{namespaceName}.{typeName}");
            if (myClassType != null)
            {
                AssetImporter importerInstance = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(InputActionAsset));
                FieldInfo fieldInfo = myClassType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo != null)
                {
                    return (string)fieldInfo.GetValue(importerInstance);
                }
            }
            
            Debug.LogError($"Couldn't find field {fieldName}");
            return string.Empty;
        }
        
        public static void ClearGeneratedFolder()
        {
            string fullSystemPath = Application.dataPath + GeneratedMapsPath;
            if (!Directory.Exists(fullSystemPath))
            {
                Directory.CreateDirectory(fullSystemPath);
            }
            else
            {
                string[] filePaths = Directory.GetFiles(fullSystemPath);

                foreach (string filePath in filePaths)
                {
                    File.Delete(filePath);
                }
            }
        }

        public static void WriteLinesToFile(List<string> newLines, string filePath)
        {
            try
            {
                using (StreamWriter sw = new(filePath))
                {
                    foreach (string line in newLines)
                    {
                        sw.WriteLine(line);
                    }
                }

                Debug.Log($"{filePath} written successfully!");
            }
            catch (Exception e)
            {
                Debug.Log($"File could not be written: {e.Message}");
            }
        }
        
        public static IEnumerable<string> GetCleanedMapNames(InputActionAsset asset)
        {
            return asset.actionMaps.Select(map => map.name.AllWhitespaceTrimmed().CapitalizeFirst());
        }

        public static string GetTemplateFilePath()
        {
            return Application.dataPath + MapActionsTemplatePath;
        }
        
        public static string GetPathForGeneratedMap(string mapName)
        {
            return Application.dataPath + GeneratedMapsPath + mapName.AllWhitespaceTrimmed() + ".cs";
        }
        
        public static bool IsMarkerStart(string line, out string markerName)
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

        public static bool IsMarkerEnd(string line)
        {
            string trimmedLine = line.Trim();
            return trimmedLine.StartsWith(MARKER) && trimmedLine.EndsWith(END);
        }
    }
}