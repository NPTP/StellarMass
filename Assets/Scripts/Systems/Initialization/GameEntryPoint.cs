using System.Collections;
using StellarMass.Systems.SceneLoading;
using StellarMass.Utilities.References;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StellarMass.Systems.Initialization
{
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private SceneReference firstScene;
        
#if UNITY_EDITOR
        private const string EditorOpenSceneKey = "EditorOpenScene";
        private const string BootstrapSceneAssetPath = "Assets/Scenes/BootstrapScene.unity";
        
        [InitializeOnLoadMethod]
        private static void LoadCorrectScene()
        {
            EditorApplication.playModeStateChanged += handlePlayModeStateChanged;

            void handlePlayModeStateChanged(PlayModeStateChange playModeStateChange)
            {
                if (playModeStateChange != PlayModeStateChange.ExitingEditMode)
                    return;
                
                int openSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
                EditorPrefs.SetInt(EditorOpenSceneKey, openSceneBuildIndex == 0 ? 1 : openSceneBuildIndex);
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapSceneAssetPath);
            }
        }
#endif

        /// <summary>
        /// Right here is the entire game's entry point. Supreme control is created!
        /// </summary>
        private IEnumerator Start()
        {
            // Initialize systems here in desired order.
            // ...

            // Wait 1 frame to allow updates to occur in systems that require a frame to update.
            yield return null;
            
#if UNITY_EDITOR
            // Disallow recursive loading of the bootstrapper from the bootstrapper itself.
            int buildIndex = Mathf.Max(1, EditorPrefs.GetInt(EditorOpenSceneKey, firstScene.BuildIndex));
#else
            int buildIndex = firstScene.BuildIndex;
#endif
            
            SceneLoader.LoadScene(buildIndex);
        }
    }
}