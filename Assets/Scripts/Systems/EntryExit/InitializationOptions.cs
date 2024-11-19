using Summoner.Systems.Data;
using Summoner.Systems.SceneManagement;
using UnityEngine;

namespace Summoner.Systems.EntryExit
{
    public class InitializationOptions : DataScriptable
    {
        [SerializeField] private bool hideCursor = true;
        public bool HideCursor => hideCursor;
     
        [SerializeField] private SceneReference firstScene;
        public SceneReference FirstScene => firstScene;
        
        [Header("FMOD Buses")]
        
        [SerializeField] private string masterBusPath = "bus:/";
        public string MasterBusPath => masterBusPath;

        [SerializeField] private string musicBusPath = "bus:/Music";
        public string MusicBusPath => musicBusPath;

        [SerializeField] private string sfxBusPath = "bus:/SFX";
        public string SfxBusPath => sfxBusPath;
    }
}