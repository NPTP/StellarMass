using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StellarMass.Systems.SceneManagement
{
    public static class SceneLoader
    {
        public static event Action<Scene> OnSceneUnloaded;
        public static event Action<Scene> OnSceneLoaded;
        
        private static Scene currentlyLoadedAdditiveScene;
        private static bool sceneLoadingInProgress;

        public static void LoadScene(SceneReference sceneReference)
        {
            LoadScene(sceneReference.BuildIndex);
        }
        
        public static void LoadScene(int buildIndex)
        {
            if (sceneLoadingInProgress)
            {
                Debug.LogError($"Tried to load scene while a scene load was already in progress. Either fix that, or make {nameof(SceneLoader)} support interrupting loads!");
                return;
            }
            
            sceneLoadingInProgress = true;
            
            if (!currentlyLoadedAdditiveScene.isLoaded)
            {
                loadNextScene();
                return;
            }

            Scene unloadingScene = currentlyLoadedAdditiveScene;
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(unloadingScene);
            unloadOperation.completed += op =>
            {
                OnSceneUnloaded?.Invoke(unloadingScene);
                loadNextScene();
            };
            
            void loadNextScene()
            {
                SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
                currentlyLoadedAdditiveScene = SceneManager.GetSceneByBuildIndex(buildIndex);
                sceneLoadingInProgress = false;
                OnSceneLoaded?.Invoke(currentlyLoadedAdditiveScene);
            }
        }
    }
}