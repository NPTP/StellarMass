using Summoner.Systems.Data;
using Summoner.Systems.SceneManagement;
using UnityEngine;

namespace Summoner.Systems.EntryExit
{
    public class InitializationOptions : DataScriptable
    {
        [SerializeField] private bool hideCursor = true;
        public bool HideCursor => hideCursor;
     
        [SerializeField] private bool autoLoadFirstScene = true;
        public bool AutoLoadFirstScene => autoLoadFirstScene;

        [SerializeField] private SceneReference firstScene;
        public SceneReference FirstScene => firstScene;
    }
}