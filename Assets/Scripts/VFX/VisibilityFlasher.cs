using UnityEngine;

namespace StellarMass.VFX
{
    public class VisibilityFlasher : MonoBehaviour
    {
        [SerializeField] private Renderer[] rdrs;
        [SerializeField] private bool isActive = true;
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

        public void Activate() => isActive = true;
        public void Deactivate() => isActive = false;

        private void OnValidate()
        {
            rdrs = GetComponentsInChildren<Renderer>();
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            if (usePattern)
            {
                char patternChar = pattern[patternIndex];
                switch (patternChar)
                {
                    case 'm':
                        MakeVisible();
                        break;
                    case 'a':
                        MakeInvisible();
                        break;
                }
                currentStateDuration += Time.deltaTime;

                if (currentStateDuration >= charTime)
                {
                    currentStateDuration = 0;
                    patternIndex = (patternIndex + 1) % pattern.Length;
                }
            }
            else
            {
                if (isOn) MakeVisible();
                else MakeInvisible();
                
                currentStateDuration += Time.deltaTime;

                if (currentStateDuration >= (isOn ? onTime : offTime))
                {
                    currentStateDuration = 0;
                    isOn = !isOn;
                }
            }
        }

        private void MakeVisible()
        {
            foreach (Renderer rdr in rdrs)
            {
                rdr.enabled = true;
            }
        }

        private void MakeInvisible()
        {
            foreach (Renderer rdr in rdrs)
            {
                rdr.enabled = false;
            }
        }
    }
}