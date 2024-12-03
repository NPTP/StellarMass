using FMODUnity;
using Summoner.Systems.Data;
using Summoner.Systems.SceneManagement;
using UnityEngine;

namespace Summoner.Systems.EntryExit
{
    public class InitializationOptions : DataScriptable
    {
        [SerializeField] private bool hideCursor = true;
        public bool HideCursor => hideCursor;

        [SerializeField] private EventReference splashScreenFmodEvent;
        public EventReference SplashScreenFmodEvent => splashScreenFmodEvent;

        [SerializeField] private bool loadFirstSceneInEditor = true;
        public bool LoadFirstSceneInEditor => loadFirstSceneInEditor;

        [SerializeField] private SceneReference firstScene;
        public SceneReference FirstScene => firstScene;
    }
}