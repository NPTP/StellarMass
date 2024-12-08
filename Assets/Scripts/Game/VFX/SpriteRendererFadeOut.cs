using Summoner.Systems.ObjectPooling;
using Summoner.Utilities;
using Summoner.Utilities.Extensions;
using UnityEngine;

namespace Summoner.Game.VFX
{
    public class SpriteRendererFadeOut : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private float elapsed;
        private float from;
        private float duration;
        private Curve curve;
        
        private void Update()
        {
            spriteRenderer.color = spriteRenderer.color.SetValues(a: from - (curve.Evaluate(elapsed / duration) * from));
            elapsed += Time.deltaTime;
            if (elapsed >= duration)
            {
                ObjectPooler.Pool(gameObject);
            }
        }

        public void Initialize(float fromParam, float durationParam, Curve.Type curveType)
        {
            from = fromParam;
            duration = durationParam;
            curve = curveType;
            elapsed = 0;
        }
    }
    
    public static class SpriteRendererFadeOutExtensions
    {
        public static void FadeOut(this GameObject gameObject, float from, float duration, Curve.Type curveType)
        {
            if (!gameObject.TryGetComponent(out SpriteRendererFadeOut spriteRendererFadeOut))
            {
                return;
            }
            
            spriteRendererFadeOut.Initialize(from, duration, curveType);
        }
    }
}