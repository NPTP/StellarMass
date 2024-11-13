using System;
using System.IO;
using UnityEngine;

namespace Summoner.Editor.CustomBuildUtilities
{
    internal class FileReadWrite
    {
        internal static void Write(string content, string path, bool isAssetsPath)
        {
            string updatedPath = path;
            if (isAssetsPath)
            {
                updatedPath = Application.dataPath.Replace("Assets", string.Empty) + path;
            }

            try
            {
                int sepIndex = updatedPath.LastIndexOf(Path.DirectorySeparatorChar);
                if (sepIndex >= 0)
                {
                    string directoryPath = updatedPath.Remove(sepIndex);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }

                File.WriteAllText(updatedPath, content);
                Debug.Log($"{updatedPath} written successfully!");
            }
            catch (Exception e)
            {
                Debug.Log($"File could not be written: {e.Message}");
            }
        }

        internal static bool TryRead(string path, out string fileText)
        {
            try
            {
                fileText = File.ReadAllText(path);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"File could not be read: {e.Message}");
                fileText = string.Empty;
                return false;
            }
        }
    }
}