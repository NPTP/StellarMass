using System;
using System.Collections.Generic;
using System.IO;
using StellarMass.Systems.Data.Persistent;
using StellarMass.Utilities;
using StellarMass.Utilities.Extensions;
using UnityEngine;

namespace StellarMass.Systems.SaveAndLoad
{
    /// <summary>
    /// Saves & loads any type of data using an ID identifier.
    /// Use for game saves, player settings, etc.
    /// </summary>
    public static class SaveLoad
    {
        private const string SAVE_PAD = "YjQaLfWUskRQHP4lO9eWbsKWJGzKwwoK";
        private const string SAVE_FILE_EXTENSION = "sum";

        private static Dictionary<int, SaveData> activeSaveData;
        private static bool initialized;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            activeSaveData = new Dictionary<int, SaveData>();
            RuntimeSafeEditorUtility.RegisterApplicationQuittingCallback(HandleApplicationQuitting);
            
            initialized = true;
        }

        private static void HandleApplicationQuitting()
        {
            if (PersistentData.Core.SaveOnApplicationExit)
                activeSaveData.Values.ForEach(saveData => saveData.Save(modifyActiveSaveData: false));
        }

        public static void Save(this SaveData saveData, bool modifyActiveSaveData = true)
        {
            if (saveData == null)
            {
                return;
            }
            
            try
            {
                if (modifyActiveSaveData)
                    activeSaveData.TryAdd(saveData.id, saveData);
                
                string json = JsonUtility.ToJson(saveData, prettyPrint: true);
                
                if (saveData.ScrambleData)
                    json = XorScramblerUtility.Scramble(json, SAVE_PAD);

                File.WriteAllText(GetSaveDataPath(saveData.id), json);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public static void Unload(this SaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }
            
            activeSaveData.Remove(saveData.id);
            saveData.Reset();
        }

        /// <summary>
        /// Ignore the id parameter when there's only one of something.
        /// E.g. a game with only one game save, or one set of game settings.
        /// Otherwise use the id to specify which instance of SaveData you're looking for.
        /// </summary>
        public static T Get<T>(int id = 0) where T : SaveData
        {
            if (activeSaveData.TryGetValue(id, out SaveData value))
            {
                return value as T;
            }

            if (!TryLoad(id, out T saveData))
            {
                saveData = Activator.CreateInstance<T>();
            }
            
            activeSaveData[id] = saveData;
            return saveData;
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
                string text = File.ReadAllText(GetSaveDataPath(id));

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

        private static string GetSaveDataPath(int id)
        {
            return $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{nameof(SaveData)}_{id}.{SAVE_FILE_EXTENSION}";
        }
    }
}