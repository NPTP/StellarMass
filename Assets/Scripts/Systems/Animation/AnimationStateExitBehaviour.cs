using System;
using UnityEngine;

namespace Summoner.Game.Splash
{
    public class AnimationStateExitBehaviour : StateMachineBehaviour
    {
        public event Action OnAnimationStateExit;
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnAnimationStateExit?.Invoke();
        }
    }
}