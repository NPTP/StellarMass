using StellarMass.InputManagement.MapInstances;
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
    }
}