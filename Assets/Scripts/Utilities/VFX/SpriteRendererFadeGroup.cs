using System;
using System.Collections.Generic;
using Summoner.Utilities.CurveUtilities;
using Summoner.Utilities.Extensions;
using UnityEngine;

namespace Summoner.Utilities.VFX
{
    public class SpriteRendererFadeGroup : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;
        [SerializeField] private float alpha = 1;
        
        public float Alpha
        {
            get => alpha;
            set
            {
                alpha = value;
                foreach (KeyValuePair<SpriteRenderer,float> keyValuePair in spriteRendererToInitialAlpha)
                {
                    keyValuePair.Key.SetAlpha(spriteRendererToInitialAlpha[keyValuePair.Key] * value);
                }
            }
        }
        
        private readonly Dictionary<SpriteRenderer, float> spriteRendererToInitialAlpha = new();
        
        private bool hasSetInitialAlphaValues;
        private float fadeTimeElapsed;
        private float fadeTo;
        private float fadeFrom;
        private float sign;
        private float scaling;
        private float fadeDuration;
        private Action onCompleteCallback;
        private bool running;
        private Curve curve;

        private void OnValidate()
        {
            alpha = Mathf.Clamp(alpha, 0, 1);
        }

        private void Awake()
        {
            SetInitialAlphaValues();
        }
        
        private void Update()
        {
            if (!running)
            {
                return;
            }

            Alpha = fadeFrom + sign * curve.Evaluate(fadeTimeElapsed / fadeDuration) * scaling;

            fadeTimeElapsed += Time.deltaTime;
            
            if (fadeTimeElapsed >= fadeDuration)
            {
                Alpha = fadeTo;
                running = false;
                onCompleteCallback?.Invoke();
            }
        }

        public SpriteRendererFadeGroup Fade(float to, float duration, Action onComplete = null)
        {
            if (to == Alpha)
            {
                return this;
            }
            
            fadeTimeElapsed = 0;
            fadeTo = to;
            From(Alpha);
            fadeDuration = duration;
            onCompleteCallback = onComplete;
            running = true;
            curve = Curve.Default;
            
            return this;
        }

        public SpriteRendererFadeGroup From(float from)
        {
            fadeFrom = from;
            sign = Mathf.Sign(fadeTo - fadeFrom);
            scaling = sign > 0 ? 1 - fadeFrom : fadeFrom;
            
            return this;
        }

        public SpriteRendererFadeGroup SetCurve(CurveType curveType)
        {
            curve = curveType;
            
            return this;
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
