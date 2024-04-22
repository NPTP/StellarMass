using UnityEngine;

namespace StellarMass.VFX
{
    public class VisibilityFlasher : RendererController
    {
        [SerializeField] private bool flashingActive = true;
        [Space]
        [SerializeField] private float onTime;
        [SerializeField] private float offTime;
        [Space]
        [SerializeField] private bool usePattern;
        [SerializeField] private float charTime = 0.1f;
        [SerializeField] private string pattern = "mmamammmmammamamaaamammma";
        
        private bool isOn;
        private float currentStateDuration;
        private int patternIndex;

        public void ActivateFlashing() => flashingActive = true;
        public void DeactivateFlashing() => flashingActive = false;

        private void Update()
        {
            if (!flashingActive)
            {
                return;
            }

            if (usePattern)
            {
                char patternChar = pattern[patternIndex];
                switch (patternChar)
                {
                    case 'm':
                        EnableRenderers();
                        break;
                    case 'a':
                        DisableRenderers();
                        break;
                }
                currentStateDuration += Time.unscaledDeltaTime;

                if (currentStateDuration >= charTime)
                {
                    currentStateDuration = 0;
                    patternIndex = (patternIndex + 1) % pattern.Length;
                }
            }
            else
            {
                if (isOn) EnableRenderers();
                else DisableRenderers();
                
                currentStateDuration += Time.unscaledDeltaTime;

                if (currentStateDuration >= (isOn ? onTime : offTime))
                {
                    currentStateDuration = 0;
                    isOn = !isOn;
                }
            }
        }
    }
}