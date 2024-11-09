using System;
using System.Collections.Generic;
using System.IO;
using Summoner.Utilities.Extensions;
using Summoner.Systems.Data.Persistent;
using Summoner.Utilities;
using UnityEngine;

namespace Summoner.Systems.SaveAndLoad
{
    /// <summary>
    /// Saves & loads any type of data using an ID identifier.
    /// Use for game saves, player settings, etc.
    /// </summary>
    public static class SaveLoad
    {
        private const string SAVE_PAD = "YjQaLfWUskRQHP4lO9eWbsKWJGzKwwoK";
        private const string SAVE_FILE_EXTENSION = "sum";

        private static Dictionary<Type, Dictionary<int, SaveData>> activeSaveData;
        private static bool initialized;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            activeSaveData = new Dictionary<Type, Dictionary<int, SaveData>>();
            RuntimeSafeEditorUtility.RegisterApplicationQuittingCallback(HandleApplicationQuitting);
            
            initialized = true;
        }

        private static void HandleApplicationQuitting()
        {
            if (PersistentData.Core.SaveOnApplicationExit)
                SaveAll();
        }

        public static void SaveAll()
        {
            activeSaveData.Values.ForEach(subDict =>
                subDict.Values.ForEach(saveData => saveData.Save()));
        }

        public static void Save(this SaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }
            
            try
            {
                string json = JsonUtility.ToJson(saveData, prettyPrint: true);
                
                if (saveData.ScrambleData)
                    json = XorScramblerUtility.Scramble(json, SAVE_PAD);

                File.WriteAllText(GetSaveDataPath(saveData.GetType(), saveData.id), json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void Unload(this SaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }

            if (activeSaveData.TryGetValue(saveData.GetType(), out Dictionary<int, SaveData> subDict))
            {
                subDict.Remove(saveData.id);
            }
            
            saveData.Reset();
        }

        /// <summary>
        /// Ignore the id parameter when there's only one of something.
        /// E.g. a game with only one game save, or one set of game settings.
        /// Otherwise use the id to specify which instance of SaveData you're looking for.
        /// </summary>
        public static T Get<T>(int id = 0) where T : SaveData
        {
            if (activeSaveData.TryGetValue(typeof(T), out Dictionary<int, SaveData> subDict) &&
                subDict.TryGetValue(id, out SaveData saveData))
            {
                return saveData as T;
            }

            if (!TryLoad(id, out T saveDataGeneric))
            {
                saveDataGeneric = Activator.CreateInstance<T>();
            }

            subDict = new Dictionary<int, SaveData> { [id] = saveDataGeneric };
            activeSaveData[typeof(T)] = subDict;
            return saveDataGeneric;
        }

        /// <summary>
        /// Just another syntax for the above Get method.
        /// </summary>
        public static void Get<T>(out T saveData, int id = 0) where T : SaveData
        {
            saveData = Get<T>();
        }

        private static bool TryLoad<T>(int id, out T saveData) where T : SaveData
        {
            string filePath = GetSaveDataPath(typeof(T), id);
            
            try
            {
                string text = File.ReadAllText(filePath);

                // Try to read Json from text as-is.
                try
                {
                    saveData = JsonUtility.FromJson<T>(text);
                    return true;
                }
                catch (Exception)
                {
                    Debug.Log("Couldn't read from JSON as-is. Attempting unscramble.");
                    
                    // If as-is didn't work, try to unscramble it, then read it.
                    try
                    {
                        text = XorScramblerUtility.Scramble(text, SAVE_PAD);
                        saveData = JsonUtility.FromJson<T>(text);
                        return true;
                    }
                    catch (Exception)
                    {
                        Debug.Log("Couldn't read unscrambled JSON.");
                    }
                }
            }
            catch (Exception)
            {
                Debug.Log($"Couldn't read JSON at {filePath}.");
            }

            saveData = null;
            return false;
        }

        private static string GetSaveDataPath(Type type, int id)
        {
            return $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{type.Name}_{id}.{SAVE_FILE_EXTENSION}";
        }
    }
}