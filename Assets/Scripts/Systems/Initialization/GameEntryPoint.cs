using System.Collections;
using StellarMass.Game.SaveLoad;
using StellarMass.Systems.Coroutines;
using StellarMass.Systems.ReferenceTable;
using StellarMass.Systems.SaveLoad;
using StellarMass.Systems.SceneManagement;
using StellarMass.Utilities.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Input = NPTP.InputSystemWrapper.Input;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace StellarMass.Systems.Initialization
{
    public class GameEntryPoint : MonoBehaviour
    { 
        [ExpandableScriptable][SerializeField]
        private InitializationOptions initializationOptions;
        
        /// <summary>
        /// This method executes as the entry point of the entire game in the bootstrap scene.
        /// This will run before ANY other developer code for this game.
        /// Afterwards the first scene specified in the persistent game data will be loaded.
        /// </summary>
        private void EntryPointExecute()
        {
            Cursor.visible = !initializationOptions.HideCursor;
            MonoReferenceTable.Initialize();
            Input.Initialize();
            CoroutineOwner.Initialize();

            StellarMassSaveData saveData = Saves.Get<StellarMassSaveData>(0);
            Debug.Log($"saveData : {saveData.z}");
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
        
        private IEnumerator Start()
        {
            // Execute game logic entry point.
            EntryPointExecute();

            // Wait 1 frame for systems that require a frame to update/set up.
            yield return null;
            
#if UNITY_EDITOR
            // Disallow recursive loading of the bootstrapper from the bootstrapper itself.
            int buildIndex = Mathf.Max(1, EditorPrefs.GetInt(EDITOR_OPEN_SCENE_KEY, initializationOptions.FirstScene.BuildIndex));
#else
            int buildIndex = firstScene.BuildIndex;
#endif
            
            SceneLoader.LoadScene(buildIndex);
            Destroy(gameObject);
        }
        #endregion
    }
}