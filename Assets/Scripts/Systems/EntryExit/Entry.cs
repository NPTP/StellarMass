using System.Collections;
using Summoner.Systems.AudioSystem;
using Summoner.Systems.Camera;
using Summoner.Systems.Coroutines;
using Summoner.Systems.MonoReferences;
using Summoner.Systems.PlayerLoop;
using Summoner.Systems.SaveAndLoad;
using Summoner.Systems.SceneManagement;
using Summoner.Systems.TimeControl;
using Summoner.Utilities.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Input = NPTP.InputSystemWrapper.Input;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Summoner.Systems.EntryExit
{
    public class Entry : MonoBehaviour
    { 
        private const int BOOTSTRAP_SCENE_BUILD_INDEX = 0;
        
        [ExpandableScriptable][SerializeField]
        private InitializationOptions initializationOptions;

        [Header("Manual Initializations")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private CoroutineOwner coroutineOwner;

        /// <summary>
        /// This coroutine executes as the entry point of the entire game (so long as no scripts have their own Awake
        /// implementation in the bootstrap scene). This will run before ANY other developer code for this game, except
        /// perhaps for some static constructors initializing members.
        /// Afterwards the first scene specified in the persistent game data will be loaded.
        /// </summary>
        private void ExecuteOnAwake()
        {
            Cursor.visible = !initializationOptions.HideCursor;
            
            SaveLoad.Initialize();
            Audio.Initialize();
            Input.Initialize();
            MonoReferenceTable.Initialize();
            TimeScale.Initialize();
            PlayerLoopUtility.Initialize();
            
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

                // Get first loaded non-bootstrap scene.
                int openSceneBuildIndex = BOOTSTRAP_SCENE_BUILD_INDEX + 1;
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.buildIndex != BOOTSTRAP_SCENE_BUILD_INDEX && scene.isLoaded)
                    {
                        openSceneBuildIndex = scene.buildIndex;
                        break;
                    }
                }
                
                EditorPrefs.SetInt(EDITOR_OPEN_SCENE_KEY, openSceneBuildIndex);
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BOOTSTRAP_SCENE_ASSET_PATH);
            }
        }
#endif

        private void Awake() => StartCoroutine(AwakeRoutine());
        private IEnumerator AwakeRoutine()
        {
            ExecuteOnAwake();
            
            // Wait 1 frame for systems that require a frame to update/set up.
            yield return null;
            
#if UNITY_EDITOR
            int firstSceneBuildIndex = EditorPrefs.GetInt(EDITOR_OPEN_SCENE_KEY, initializationOptions.FirstScene.BuildIndex);
#else
            int firstSceneBuildIndex = initializationOptions.FirstScene.BuildIndex;
#endif
            // Disallow recursive loading of the bootstrapper from the bootstrapper itself.
            if (firstSceneBuildIndex == BOOTSTRAP_SCENE_BUILD_INDEX) firstSceneBuildIndex++;

#if UNITY_EDITOR
            if (initializationOptions.LoadFirstSceneInEditor)
#endif
                SceneLoader.LoadScene(firstSceneBuildIndex, instant: true);

            Destroy(gameObject);
            Resources.UnloadUnusedAssets();
        }
        #endregion
    }
}