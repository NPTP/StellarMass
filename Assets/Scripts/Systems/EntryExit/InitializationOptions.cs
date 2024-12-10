using FMODUnity;
using Summoner.Systems.Data;
using Summoner.Systems.SceneManagement;
using UnityEngine;

namespace Summoner.Systems.EntryExit
{
    public class InitializationOptions : DataScriptable
    {
        [SerializeField] private bool hideCursorInPlayer = true;
        public bool HideCursorInPlayer => hideCursorInPlayer;
        
#if UNITY_EDITOR
        [SerializeField] private bool hideCursorInEditor = false;
        public bool HideCursorInEditor => hideCursorInEditor;
#endif
        
        [SerializeField] private EventReference splashScreenFmodEvent;
        public EventReference SplashScreenFmodEvent => splashScreenFmodEvent;

        [SerializeField] private bool loadFirstSceneInEditor = true;
        public bool LoadFirstSceneInEditor => loadFirstSceneInEditor;

        [SerializeField] private SceneReference firstScene;
        public SceneReference FirstScene => firstScene;
    }
}