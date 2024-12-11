using Summoner.Systems.SceneManagement;
using UnityEngine;

namespace Summoner.Systems.Data.Persistent
{
    [CreateAssetMenu]
    public sealed class ScenesPersistentData : PersistentDataContainer
    {
        [SerializeField] private SceneReference bootstrap;
        public SceneReference Bootstrap => bootstrap;
        
        [SerializeField] private SceneReference splash;
        public SceneReference Splash => splash;
        
        [SerializeField] private SceneReference game;
        public SceneReference Game => game;
    }
}