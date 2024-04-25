using StellarMass.Utilities;
using StellarMass.Utilities.Extensions;
using UnityEngine;

namespace StellarMass.Ship
{
    public class SpriteRendererTrail : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float fadeTime = 0.4f;

        private float timeElapsed = 0;
        private float currentAlpha = 1;
        
        private void Update()
        {
            if (timeElapsed >= fadeTime)
            {
                Destroy(gameObject);
            }
            
            spriteRenderer.SetAlpha(currentAlpha);
            currentAlpha = Mathf.Lerp(0.75f, 0, Curves.EaseOutExp(timeElapsed / fadeTime));

            timeElapsed += Time.deltaTime;
        }
    }
}