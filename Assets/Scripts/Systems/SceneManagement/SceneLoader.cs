using UnityEngine;
using UnityEngine.SceneManagement;

namespace StellarMass.Systems.SceneManagement
{
    public static class SceneLoader
    {
        private static Scene currentlyLoadedAdditiveScene;

        public static void LoadScene(SceneReference sceneReference)
        {
            LoadScene(sceneReference.BuildIndex);
        }
        
        public static void LoadScene(int buildIndex)
        {
            if (!currentlyLoadedAdditiveScene.isLoaded)
            {
                loadScene();
                return;
            }
            
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentlyLoadedAdditiveScene);
            unloadOperation.completed += op => loadScene();
            
            void loadScene()
            {
                SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
                currentlyLoadedAdditiveScene = SceneManager.GetSceneByBuildIndex(buildIndex);
            }
        }
    }
}