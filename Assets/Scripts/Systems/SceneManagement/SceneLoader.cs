using System;
using System.Collections;
using Summoner.Systems.Coroutines;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Summoner.Systems.SceneManagement
{
    public static class SceneLoader
    {
        private const float LOADING_PROGRESS_SCENE_ACTIVATION_MAGIC_NUMBER = 0.9f;
        public static event Action OnStartedLoading;
        public static event Action<Scene> OnSceneUnloadCompleted;
        public static event Action<Scene> OnSceneLoadCompleted;

        public static Scene CurrentScene { get; private set; }
        public static float LoadingProgress => isLoading && loadSceneOperation != null ? loadSceneOperation.progress : 0;
        
        private static bool isLoading;
        private static AsyncOperation loadSceneOperation;

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
                CoroutineOwner.StartRoutine(loadNextScene());
                return;
            }

            Scene unloadingScene = CurrentScene;
            AsyncOperation unloadSceneOperation = SceneManager.UnloadSceneAsync(unloadingScene);
            unloadSceneOperation.completed += op =>
            {
                OnSceneUnloadCompleted?.Invoke(unloadingScene);
                CoroutineOwner.StartRoutine(loadNextScene());
            };
            
            IEnumerator loadNextScene()
            {
                loadSceneOperation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
                loadSceneOperation.allowSceneActivation = false;
                
                while (!loadSceneOperation.isDone)
                {
                    if (loadSceneOperation.progress >= LOADING_PROGRESS_SCENE_ACTIVATION_MAGIC_NUMBER &&
                        !loadSceneOperation.allowSceneActivation)
                    {
                        loadSceneOperation.allowSceneActivation = true;
                    }

                    yield return null;
                }

                loadSceneOperation = null;
                CurrentScene = SceneManager.GetSceneByBuildIndex(buildIndex);
                isLoading = false;
                OnSceneLoadCompleted?.Invoke(CurrentScene);
            }
        }
    }
}