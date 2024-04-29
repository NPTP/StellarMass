using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace StellarMass.InputManagement.Editor
{
    public static class GeneratorHelper
    {
        private const string MARKER = "// MARKER";
        private const string START = "Start";
        private const string END = "End";
        
        private static char S => Path.DirectorySeparatorChar;
        private static string MapInstancesPath => $@"{S}Scripts{S}InputManagement{S}MapInstances{S}";

        public static void WriteLinesToFile(List<string> newLines, string filePath)
        {
            try
            {
                using (StreamWriter sw = new(filePath))
                    foreach (string line in newLines)
                        sw.WriteLine(line);

                Debug.Log($"{filePath} written successfully!");
            }
            catch (Exception e)
            {
                Debug.Log($"File could not be written: {e.Message}");
            }
        }

        public static string GetFilePathForMapName(string mapName)
        {
            return Application.dataPath + MapInstancesPath + mapName + ".cs";
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