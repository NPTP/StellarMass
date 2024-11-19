using UnityEngine;

namespace Summoner.Game.GameControl
{
    public class PauseMenuItem : MonoBehaviour
    {
        [SerializeField] private Transform itemTransform;
        public Transform ItemTransform => itemTransform;
        
        [SerializeField] private Transform arrowPositionTransform;
        public Transform ArrowPositionTransform => arrowPositionTransform;

        [SerializeField] private PauseMenuItem up;
        public PauseMenuItem Up => up;
        
        [SerializeField] private PauseMenuItem down;
        public PauseMenuItem Down => down;
    }
}