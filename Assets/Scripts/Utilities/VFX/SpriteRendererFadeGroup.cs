using System;
using System.Collections.Generic;
using DG.Tweening;
using Summoner.Utilities.Extensions;
using UnityEngine;

namespace Summoner.Utilities.VFX
{
    public class SpriteRendererFadeGroup : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;
        
        private readonly Dictionary<SpriteRenderer, float> spriteRendererToInitialAlpha = new();

        private bool hasSetInitialAlphaValues;
        
        // TODO: Drop the tweens and adopt the Update way that SpriteRendererFadeOut uses
        private Sequence fadeSequence;

        private void Awake()
        {
            SetInitialAlphaValues();
        }

        public void ResetToInitialAlpha()
        {
            SetInitialAlphaValues();
            spriteRenderers.For(sr => sr.color = sr.color.SetValues(a: spriteRendererToInitialAlpha[sr]));
        }

        public void Fade(float to, float duration, Action onComplete = null)
        {
            fadeSequence?.Kill();
            fadeSequence = DOTween.Sequence();
            spriteRenderers.For(sr => fadeSequence.Insert(0, sr.DOFade(to, duration)));

            if (onComplete != null)
            {
                fadeSequence.onComplete = onComplete.Invoke;
            }
        }
        
        public void Fade(float to, float duration, Ease ease, Action onComplete = null)
        {
            fadeSequence?.Kill();
            fadeSequence = DOTween.Sequence();
            spriteRenderers.For(sr => fadeSequence.Insert(0, sr.DOFade(to, duration).SetEase(ease)));

            if (onComplete != null)
            {
                fadeSequence.onComplete = onComplete.Invoke;
            }
        }
        
        private void SetInitialAlphaValues()
        {
            if (hasSetInitialAlphaValues)
            {
                return;
            }

            spriteRenderers.For(sr => spriteRendererToInitialAlpha.Add(sr, sr.color.a));
            hasSetInitialAlphaValues = true;
        }

#if UNITY_EDITOR
        public void EDITOR_GetComponentsInChildren()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
#endif
    }
}
