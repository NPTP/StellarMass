using StellarMass.Systems.Data;
using StellarMass.Systems.SceneManagement;
using UnityEngine;

namespace StellarMass.Systems.Initialization
{
    public class InitializationOptions : DataScriptable
    {
        [SerializeField] private bool hideCursor = true;
        public bool HideCursor => hideCursor;
     
        [SerializeField] private SceneReference firstScene;
        public SceneReference FirstScene => firstScene;
    }
}