using System.Collections;
using Summoner.Utilities.Attributes;
using UnityEngine;

namespace Summoner.Systems.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorController : MonoBehaviour
    {
        public event System.Action OnAnimationCompleted;
        
        [SerializeField][Required] private Animator animator;

        private Coroutine animationCoroutine;

        public void Play(int stateNameHash)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(PlayRoutine(stateNameHash));
        }
        
        private IEnumerator PlayRoutine(int stateNameHash)
        {
            animator.CrossFade(stateNameHash, 0);
            
            // Frame required to be "in" new animation
            yield return null;
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.loop)
            { 
                yield break;
            }

            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }

            animator.StopPlayback();
            animationCoroutine = null;
            OnAnimationCompleted?.Invoke();
        }
    }
}