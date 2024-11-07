using StellarMass.Systems.Data.Persistent;
using StellarMass.Systems.SceneManagement;
using UnityEngine;

namespace StellarMass.Game.Data.Persistent
{
    public sealed class GamePersistentData : PersistentDataContainer
    {
        [SerializeField] private bool hideCursorOnStart = true;
        public bool HideCursorOnStart => hideCursorOnStart;
     
        [SerializeField] private SceneReference firstScene;
        public SceneReference FirstScene => firstScene;
    }
}