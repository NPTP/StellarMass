using System;
using UnityEngine;

namespace Summoner.Systems.SceneManagement
{
    [Serializable]
    public class SceneReference
    {
#if UNITY_EDITOR
        [SerializeField] private string guid;
        [SerializeField] private string path;
        public string Path => path;
#endif
        
        [SerializeField] private int buildIndex;
        public int BuildIndex => buildIndex;
    }
}