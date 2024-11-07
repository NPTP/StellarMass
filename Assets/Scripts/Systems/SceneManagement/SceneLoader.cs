using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StellarMass.Systems.SceneManagement
{
    public static class SceneLoader
    {
        public static event Action OnStartedLoading;
        public static event Action<Scene> OnSceneUnloadCompleted;
        public static event Action<Scene> OnSceneLoadCompleted;

        public static Scene CurrentScene { get; private set; }
        
        private static bool isLoading;

        public static void LoadScene(SceneReference sceneReference)
        {
            LoadScene(sceneReference.BuildIndex);
        }
        
        public static void LoadScene(int buildIndex)
        {
            if (isLoading)
            {
                Debug.LogError($"Tried to load scene while a scene load was already in progress.");
                return;
            }
            
            isLoading = true;
            OnStartedLoading?.Invoke();
            
            if (!CurrentScene.isLoaded)
            {
                loadNextScene();
                return;
            }

            Scene unloadingScene = CurrentScene;
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(unloadingScene);
            unloadOperation.completed += op =>
            {
                OnSceneUnloadCompleted?.Invoke(unloadingScene);
                loadNextScene();
            };
            
            void loadNextScene()
            {
                SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
                CurrentScene = SceneManager.GetSceneByBuildIndex(buildIndex);
                isLoading = false;
                OnSceneLoadCompleted?.Invoke(CurrentScene);
            }
        }
    }
}