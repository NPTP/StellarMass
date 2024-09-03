using System;
using UnityEngine;

namespace StellarMass.Utilities.References
{
    [Serializable]
    public class SceneReference
    {
#if UNITY_EDITOR
        [SerializeField] private string guid;
#endif
        
        [SerializeField] private int buildIndex;
        public int BuildIndex => buildIndex;
    }
}