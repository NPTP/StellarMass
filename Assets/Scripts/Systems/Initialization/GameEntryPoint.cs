using System.Collections;
using StellarMass.Systems.Data.Persistent;
using StellarMass.Systems.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace StellarMass.Systems.Initialization
{
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private SceneReference firstScene;

#if UNITY_EDITOR
        private const string EDITOR_OPEN_SCENE_KEY = "EditorOpenScene";
        private const string BOOTSTRAP_SCENE_ASSET_PATH = "Assets/Scenes/Bootstrap.unity";
        
        [InitializeOnLoadMethod]
        private static void LoadCorrectScene()
        {
            EditorApplication.playModeStateChanged -= handlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += handlePlayModeStateChanged;

            void handlePlayModeStateChanged(PlayModeStateChange playModeStateChange)
            {
                if (playModeStateChange != PlayModeStateChange.ExitingEditMode)
                    return;
                
                int openSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
                EditorPrefs.SetInt(EDITOR_OPEN_SCENE_KEY, openSceneBuildIndex == 0 ? 1 : openSceneBuildIndex);
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BOOTSTRAP_SCENE_ASSET_PATH);
            }
        }
#endif

        private void Awake()
        {
            Cursor.visible = !PD.Game.HideCursorOnGameStart;
        }

        /// <summary>
        /// Right here is the entire game's entry point. Supreme control created!
        /// </summary>
        private IEnumerator Start()
        {
            // Initialize systems here in desired order.
            // ...

            // Wait 1 frame to allow updates to occur in systems that require a frame to update.
            yield return null;
            
#if UNITY_EDITOR
            // Disallow recursive loading of the bootstrapper from the bootstrapper itself.
            int buildIndex = Mathf.Max(1, EditorPrefs.GetInt(EDITOR_OPEN_SCENE_KEY, firstScene.BuildIndex));
#else
            int buildIndex = firstScene.BuildIndex;
#endif
            
            SceneLoader.LoadScene(buildIndex);
            
            Destroy(gameObject);
        }
    }
}