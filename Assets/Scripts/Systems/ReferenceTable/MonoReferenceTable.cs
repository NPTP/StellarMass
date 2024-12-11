using System;
using System.Collections.Generic;
using Summoner.Systems.SceneManagement;
using UnityEngine.SceneManagement;

namespace Summoner.Systems.MonoReferences
{
    /// <summary>
    /// Static access to currently loaded MonoBehaviours with safe TryGet methods.
    /// </summary>
    public static class MonoReferenceTable
    {
        private static Dictionary<Type, ReferenceTableMonoBehaviour> references;
        private static bool initialized;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }
            
            references = new Dictionary<Type, ReferenceTableMonoBehaviour>();
            SceneLoader.OnSceneUnloadCompleted += HandleSceneUnloadCompleted;
            
            initialized = true;
        }

        public static bool TryGet<T>(out T reference) where T : ReferenceTableMonoBehaviour
        {
            if (!initialized) Initialize();

            if (references.TryGetValue(typeof(T), out ReferenceTableMonoBehaviour value) && value is T tValue)
            {
                reference = tValue;
                return true;
            }
            
            reference = null;
            return false;
        }
        
        public static bool TryAdd<T>(T reference) where T : ReferenceTableMonoBehaviour
        {
            if (!initialized) Initialize();
            return references.TryAdd(reference.GetType(), reference);
        }

        public static bool Remove<T>(T reference) where T : ReferenceTableMonoBehaviour
        {
            if (!initialized) Initialize();
            return references.Remove(typeof(T));
        }
        
        private static void HandleSceneUnloadCompleted(Scene scene)
        {
            PurgeDeadReferences();
        }
        
        private static void PurgeDeadReferences()
        {
            if (!initialized) return;
            
            List<Type> keysToDeadReferences = new();
            
            foreach (KeyValuePair<Type, ReferenceTableMonoBehaviour> kvp in references)
            {
                if (kvp.Value == null)
                    keysToDeadReferences.Add(kvp.Key);
            }

            foreach (Type type in keysToDeadReferences)
            {
                references.Remove(type);
            }
        }
    }
}