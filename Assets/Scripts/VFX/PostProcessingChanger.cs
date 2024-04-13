using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace StellarMass.VFX
{
    public class PostProcessingChanger : MonoBehaviour
    {
        [SerializeField] private PostProcessVolume volume;

        [Header("Bloom")]
        [SerializeField] private float diffusionRange = 0.25f;
        [SerializeField] private float diffusionSpeed = 4f;

        private BloomSettings bloomSettings;

        private void Awake()
        {
            bloomSettings = new BloomSettings(volume.profile);
        }
        
        private void Update()
        {
            BloomUpdate();
        }
        
        private void BloomUpdate()
        {
            bloomSettings.Diffusion = bloomSettings.DiffusionInitialValue + (diffusionRange * Mathf.Sin(diffusionSpeed * Time.time));
        }
    }
}