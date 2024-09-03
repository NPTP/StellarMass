using UnityEngine.SceneManagement;

namespace StellarMass.Systems.SceneLoading
{
    public static class SceneLoader
    {
        private static Scene currentlyLoadedAdditiveScene;
        
        public static void LoadScene(int buildIndex)
        {
            if (currentlyLoadedAdditiveScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(currentlyLoadedAdditiveScene);
            }
            
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
        }
    }
}