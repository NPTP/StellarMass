using FMODUnity;
using Summoner.Systems.Data.Persistent;
using UnityEngine;

namespace Summoner.Game.Data
{
    public sealed class AudioPersistentData : PersistentDataContainer
    {
        [Header("Buses")]
        
        [SerializeField] private string masterBusPath = "bus:/";
        public string MasterBusPath => masterBusPath;

        [SerializeField] private string musicBusPath = "bus:/Music";
        public string MusicBusPath => musicBusPath;

        [SerializeField] private string sfxBusPath = "bus:/SFX";
        public string SfxBusPath => sfxBusPath;
        
        [Header("SFX")]
        
        [SerializeField] private EventReference ambience;
        public EventReference Ambience => ambience;
        
        [Header("SFX/Menu")]

        [SerializeField] private EventReference select;
        public EventReference Select => select;
    }
}