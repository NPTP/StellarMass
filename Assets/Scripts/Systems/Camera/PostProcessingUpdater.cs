using StellarMass.Game.VFX.PostProcessing;
using StellarMass.Systems.Data.Persistent;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace StellarMass.Systems.Camera
{
    public class PostProcessingUpdater : MonoBehaviour
    {
        private BloomSettings bloomSettings;

        public void Initialize(PostProcessVolume volume)
        {
            bloomSettings = new BloomSettings(volume.profile);
        }
        
        private void Update()
        {
            BloomUpdate();
        }
        
        private void BloomUpdate()
        {
            bloomSettings.Diffusion = bloomSettings.DiffusionInitialValue +
                                      PersistentData.Camera.DiffusionRange * Mathf.Sin(PersistentData.Camera.DiffusionSpeed * Time.time);
        }
    }
}