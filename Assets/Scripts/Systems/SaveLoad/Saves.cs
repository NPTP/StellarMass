using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StellarMass.Systems.SaveLoad
{
    public static class Saves
    {
        private static readonly Dictionary<int, SaveData> activeSaveData = new();

        public static void Save(this SaveData saveData)
        {
            try
            {
                activeSaveData.TryAdd(saveData.id, saveData);
                File.WriteAllText(GetSaveDataPath(saveData.id), JsonUtility.ToJson(saveData, prettyPrint: true));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public static void Unload(SaveData saveData)
        {
            activeSaveData.Remove(saveData.id);
            // TODO: clear/dispose of the save somehow
        }

        public static T Get<T>(int id) where T : SaveData
        {
            if (activeSaveData.TryGetValue(id, out SaveData value))
            {
                return value as T;
            }

            return Load<T>(id);
        }
        
        private static T Load<T>(int id) where T : SaveData
        {
            try
            {
                string text = File.ReadAllText(GetSaveDataPath(id));
                T saveData = JsonUtility.FromJson<T>(text);
                activeSaveData[saveData.id] = saveData;
                return saveData;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }

        private static string GetSaveDataPath(int id)
        {
            return Application.persistentDataPath + Path.DirectorySeparatorChar + id + nameof(SaveData);
        }
    }
}