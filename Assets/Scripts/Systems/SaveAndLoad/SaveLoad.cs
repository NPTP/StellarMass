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

        public static void Save<T>(this T saveData) where T : SaveData
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

                File.WriteAllText(GetSaveDataPath<T>(saveData.id), json);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public static void Unload<T>(this T saveData) where T : SaveData
        {
            if (saveData == null)
            {
                return;
            }

            if (activeSaveData.TryGetValue(typeof(T), out Dictionary<int, SaveData> subDict))
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

            if (!TryLoad(id, out saveData))
            {
                saveData = Activator.CreateInstance<T>();
            }

            subDict = new Dictionary<int, SaveData> { [id] = saveData };
            activeSaveData[typeof(T)] = subDict;
            return saveData as T;
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
            try
            {
                string text = File.ReadAllText(GetSaveDataPath<T>(id));

                // Try to read Json from text as-is.
                try
                {
                    saveData = JsonUtility.FromJson<T>(text);
                    return true;
                }
                catch (Exception fromJsonException1)
                {
                    // If as-is didn't work, try to unscramble it, then read it.
                    try
                    {
                        text = XorScramblerUtility.Scramble(text, SAVE_PAD);
                        saveData = JsonUtility.FromJson<T>(text);
                        return true;
                    }
                    catch (Exception fromJsonException2)
                    {
                        Debug.LogError(fromJsonException2);
                    }
                    
                    Debug.LogError(fromJsonException1);
                }
            }
            catch (Exception readFileException)
            {
                Debug.LogError(readFileException);
            }

            saveData = null;
            return false;
        }

        private static string GetSaveDataPath<T>(int id) where T : SaveData
        {
            return $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{typeof(T).Name}_{id}.{SAVE_FILE_EXTENSION}";
        }
    }
}