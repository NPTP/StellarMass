using System;
using StellarMass.InputManagement;
using StellarMass.InputManagement.MapInstances;
using StellarMass.OldInput;
using UnityEngine;

namespace StellarMass.Data
{
    [CreateAssetMenu]
    public class InputData : DataScriptable
    {
        // MARKER.MapInstanceSerializedFields.Start
        [SerializeField] private Gameplay gameplay;
        [SerializeField] private PauseMenu pauseMenu;
        // MARKER.MapInstanceSerializedFields.End

        [Obsolete]
        [SerializeField] private InputInfo[] inputInfos;
        public InputInfo[] InputInfos => inputInfos;
    }
}