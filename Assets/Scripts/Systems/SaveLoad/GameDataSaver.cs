using System;
using System.Collections.Generic;
using System.IO;
using StellarMass.Systems.Data.Persistent;
using StellarMass.Utilities;
using StellarMass.Utilities.Extensions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StellarMass.Systems.SaveLoad
{
    /// <summary>
    /// Saves any type of data using an ID identifier.
    /// Use for game saves, player settings, etc.
    /// </summary>
    public static class GameDataSaver
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
                activeSaveData.Values.ForEach(saveData => saveData.Save());
        }

        public static void Save(this SaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }
            
            try
            {
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

        public static T Get<T>(int id) where T : SaveData
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
        /// Use this when you don't need an ID, e.g. in a game where there's only one
        /// save game, or only one set of game settings, etc.
        /// </summary>
        public static T GetFirst<T>() where T : SaveData
        {
            return Get<T>(0);
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
            return $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{id}_{nameof(SaveData)}.{SAVE_FILE_EXTENSION}";
        }
    }
}