using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Summoner.Systems.SceneManagement;
using Summoner.Utilities.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Summoner.Systems.ObjectPooling
{
    public class ObjectPooler : ManualInitSingleton<ObjectPooler>
    {
        /// <summary>
        /// Get a list of pooled objects by feeding in the prefab as a key.
        /// </summary>
        private readonly Dictionary<Object, List<Object>> pooledObjects = new Dictionary<Object, List<Object>>();
        
        /// <summary>
        /// Get the prefab that a particular Unity Object instance is using.
        /// </summary>
        private readonly Dictionary<int, Object> prefabsByObjectInstance = new Dictionary<int, Object>();
        
        private Transform poolParent;

        #region Lifecycle
        
        protected override void InitializeOverrideable()
        {
            SceneLoader.OnSceneUnloadBegun += HandleSceneUnloadBegun;
            
            transform.position = Vector3.zero;
            poolParent = new GameObject().transform;
            poolParent.localPosition = Vector3.zero;
            UpdatePoolParentNameInEditor();
        }

        private void OnDestroy()
        {
            SceneLoader.OnSceneUnloadBegun -= HandleSceneUnloadBegun;
            
            ClearPool(calledFromOnDestroy: true);
        }
        
        private void HandleSceneUnloadBegun(Scene scene)
        {
            ClearPool(calledFromOnDestroy: false);
        }

        #endregion

        #region Instantiation
        
        public new static T Instantiate<T>(T prefab, Transform parent = null) where T : Object
        {
            return Instance.InstantiateInternal(prefab, Vector3.zero, Quaternion.identity, parent);
        }

        public new static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object
        {
            return Instance.InstantiateInternal(prefab, position, rotation, null);
        }

        private T InstantiateInternal<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            T instantiatedObject = null;

            if (pooledObjects.TryGetValue(prefab, out List<Object> objectInstances) && objectInstances.Count > 0)
            {
                int lastIndex = objectInstances.Count - 1;
                instantiatedObject = objectInstances[lastIndex] as T;
                objectInstances.RemoveAt(lastIndex);
            }
            else
            {
                instantiatedObject = InstantiateNewPrefabInstance(prefab);
            }

            GameObject instantiatedGameObject = GetGameObjectFromObject(instantiatedObject);
            instantiatedGameObject.SetActive(true);
            Transform instantiatedTransform = instantiatedGameObject.transform;
            instantiatedTransform.position = position;
            instantiatedTransform.rotation = rotation;
            instantiatedTransform.SetParent(parent);
            
            UpdatePoolParentNameInEditor();

            return instantiatedObject;
        }

        private T InstantiateNewPrefabInstance<T>(T prefab) where T : Object
        {
            T newPrefabInstance = Object.Instantiate(prefab);
            prefabsByObjectInstance.Add(newPrefabInstance.GetInstanceID(), prefab);
            return newPrefabInstance;
        }
        
        #endregion

        #region Pooling
        
        public static void Pool(Object objectToPool)
        {
            if (!Exists)
            {
                return;
            }
            
            Instance.PoolInternal(objectToPool);
        }

        private void PoolInternal(Object objectToPool)
        {
            if (objectToPool == null)
            {
                return;
            }
            
            if (!prefabsByObjectInstance.TryGetValue(objectToPool.GetInstanceID(), out Object prefab))
            {
                DestroyPoolerObject(objectToPool);
                return;
            }

            if (pooledObjects.TryGetValue(prefab, out List<Object> pooledInstances))
            {
                pooledInstances.Add(objectToPool);
            }
            else
            {
                List<Object> pooled = new List<Object> { objectToPool };
                pooledObjects[prefab] = pooled;
            }
            
            GameObject pooledGameObject = GetGameObjectFromObject(objectToPool);
            pooledGameObject.SetActive(false);
            pooledGameObject.transform.parent = poolParent;
            UpdatePoolParentNameInEditor();
        }

        private void DestroyPoolerObject(Object objectToDestroy)
        {
            if (!prefabsByObjectInstance.TryGetValue(objectToDestroy.GetInstanceID(), out Object prefab))
            {
                Destroy(GetGameObjectFromObject(objectToDestroy));
                return;
            }

            if (!pooledObjects.TryGetValue(prefab, out List<Object> objectInstances))
            {
                Destroy(GetGameObjectFromObject(objectToDestroy));
                return;
            }
            
            objectInstances.Remove(objectToDestroy);
            if (objectInstances.Count == 0)
            {
                pooledObjects.Remove(prefab);
            }
       
            prefabsByObjectInstance.Remove(objectToDestroy.GetInstanceID());
            
            Destroy(GetGameObjectFromObject(objectToDestroy));
        }
        
        #endregion

        #region Misc
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePoolParentNameInEditor()
        {
#if UNITY_EDITOR
            poolParent.name = $"Object Pool [Count: {poolParent.childCount:000}]";
#endif
        }
        
        private static GameObject GetGameObjectFromObject<T>(T tObject) where T : Object
        {
            return tObject is Component component
                ? component.gameObject
                : tObject as GameObject;
        }

        private void ClearPool(bool calledFromOnDestroy)
        {
            if (!Exists)
            {
                return;
            }

            List<Object> objectsToDestroy = new List<Object>(pooledObjects.Values.Count);
            
            foreach (List<Object> pooledObjectList in pooledObjects.Values)
            {
                foreach (Object pooledObject in pooledObjectList)
                {
                    if (pooledObject == null)
                    {
                        if (!calledFromOnDestroy)
                            Debug.LogWarning($"Trying to destroy a null object in Summoner system {nameof(ObjectPooler)}");

                        continue;
                    }
                    
                    objectsToDestroy.Add(pooledObject);
                }

                pooledObjectList.Clear();
            }

            foreach (Object o in objectsToDestroy)
            {
                DestroyPoolerObject(o);
            }
            
            pooledObjects.Clear();
            prefabsByObjectInstance.Clear();
        }

        public static void PrePopulatePool(Object prefab, int count)
        {
            GameObject prefabGameObject = GetGameObjectFromObject(prefab);
            
            // Skip if not viable or object is an instance instead of a prefab reference
            if (prefabGameObject == null || prefabGameObject.scene.rootCount != 0)
            {
                return;
            }

            Object[] instances = new Object[count];
            
            for (int i = 0; i < count; i++)
                instances[i] = Instantiate(prefab);
            
            for (int i = 0; i < count; i++)
                Pool(instances[i]);
        }

        #endregion
    }
}