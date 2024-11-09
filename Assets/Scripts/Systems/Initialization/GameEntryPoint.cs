using System.Collections;
using Summoner.Systems.AudioSystem;
using Summoner.Systems.Camera;
using Summoner.Systems.Coroutines;
using Summoner.Systems.ReferenceTable;
using Summoner.Systems.SaveAndLoad;
using Summoner.Utilities.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Input = NPTP.InputSystemWrapper.Input;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Summoner.Systems.Initialization
{
    public class GameEntryPoint : MonoBehaviour
    { 
        [ExpandableScriptable][SerializeField]
        private InitializationOptions initializationOptions;

        [Header("Manual Initializations")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private CoroutineOwner coroutineOwner;
        
        /// <summary>
        /// This method executes as the entry point of the entire game (so long as no scripts have their own Awake
        /// implementation in the bootstrap scene). This will run before ANY other developer code for this game.
        /// Afterwards the first scene specified in the persistent game data will be loaded.
        /// </summary>
        private void ExecuteOnAwake()
        {
            Cursor.visible = !initializationOptions.HideCursor;
            
            SaveLoad.Initialize();
            Audio.Initialize(initializationOptions);
            Input.Initialize();
            MonoReferenceTable.Initialize();
            
            cameraController.Initialize();
            coroutineOwner.Initialize();
        }

        #region Implementation
        
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
            ExecuteOnAwake();
        }

        private IEnumerator Start()
        {
            // Wait 1 frame for systems that require a frame to update/set up.
            yield return null;
            
#if UNITY_EDITOR
            // Disallow recursive loading of the bootstrapper from the bootstrapper itself.
            int buildIndex = Mathf.Max(1, EditorPrefs.GetInt(EDITOR_OPEN_SCENE_KEY, initializationOptions.FirstScene.BuildIndex));
#else
            int buildIndex = firstScene.BuildIndex;
#endif
            
            // Only place in the game code a Scene should be loaded by calling Unity SceneManager API directly.
            // Elsewhere, always use SceneLoader.
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
            
            Destroy(gameObject);
            Resources.UnloadUnusedAssets();
        }
        #endregion
    }
}