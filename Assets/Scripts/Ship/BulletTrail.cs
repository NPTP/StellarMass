using StellarMass.Utilities;
using StellarMass.Utilities.Extensions;
using UnityEngine;

namespace StellarMass.Ship
{
    public class BulletTrail : MonoBehaviour
    {
        private const float FadeTime = 0.4f;
        
        [SerializeField] private SpriteRenderer spriteRenderer;

        private float timeElapsed = 0;
        private float currentAlpha = 1;
        
        private void Update()
        {
            if (timeElapsed >= FadeTime)
            {
                Destroy(this.gameObject);
            }
            
            spriteRenderer.SetAlpha(currentAlpha);
            currentAlpha = Mathf.Lerp(0.75f, 0, Curves.EaseOutExp(timeElapsed / FadeTime));

            timeElapsed += Time.deltaTime;
        }
    }
}