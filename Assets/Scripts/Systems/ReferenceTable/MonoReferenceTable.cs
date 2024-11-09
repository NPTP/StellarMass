using System;
using System.Collections.Generic;
using Summoner.Systems.SceneManagement;
using UnityEngine.SceneManagement;

namespace Summoner.Systems.ReferenceTable
{
    /// <summary>
    /// Static access to currently loaded MonoBehaviours with safe TryGet methods.
    /// </summary>
    public static class MonoReferenceTable
    {
        private static Dictionary<Type, ReferenceableMonoBehaviour> references;
        private static bool initialized;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }
            
            references = new Dictionary<Type, ReferenceableMonoBehaviour>();
            SceneLoader.OnSceneUnloadCompleted += HandleSceneUnloadCompleted;
            
            initialized = true;
        }

        public static bool TryGet<T>(out T reference) where T : ReferenceableMonoBehaviour
        {
            if (!initialized) Initialize();

            if (references.TryGetValue(typeof(T), out ReferenceableMonoBehaviour value) && value is T tValue)
            {
                reference = tValue;
                return true;
            }
            
            reference = null;
            return false;
        }
        
        public static bool TryAdd<T>(T reference) where T : ReferenceableMonoBehaviour
        {
            if (!initialized) Initialize();
            
            return references.TryAdd(typeof(T), reference);
        }

        public static bool Remove<T>(T reference) where T : ReferenceableMonoBehaviour
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
            List<Type> keysToDeadReferences = new();
            
            foreach (KeyValuePair<Type, ReferenceableMonoBehaviour> kvp in references)
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