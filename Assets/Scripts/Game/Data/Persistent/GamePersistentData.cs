using StellarMass.Systems.Data.Persistent;
using UnityEngine;

namespace StellarMass.Game.Data.Persistent
{
    public sealed class GamePersistentData : PersistentDataContainer
    {
        [SerializeField] private bool hideCursor = true;
        public bool HideCursor => hideCursor;
    }
}