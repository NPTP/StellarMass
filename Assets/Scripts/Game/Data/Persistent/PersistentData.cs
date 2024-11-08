using StellarMass.Game.Data;
using StellarMass.Game.Data.Persistent;
using UnityEngine;

namespace StellarMass.Systems.Data.Persistent
{
    public sealed partial class PersistentData
    {
        [SerializeField] private GamePersistentData game;
        public static GamePersistentData Game => Instance.game;
        
        [SerializeField] private PlayerPersistentData player;
        public static PlayerPersistentData Player => Instance.player;
        
        [SerializeField] private AudioPersistentData audio;
        public static AudioPersistentData Audio => Instance.audio;
    }
}