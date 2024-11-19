using Summoner.Game.VFX.PostProcessing;
using Summoner.Systems.Data.Persistent;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Summoner.Systems.Camera.CustomUpdaters
{
    public sealed class PostProcessingUpdater : CustomUpdater
    {
        [SerializeField] private PostProcessVolume volume;
        
        private BloomSettings bloomSettings;

        protected sealed override void Initialize()
        {
            bloomSettings = new BloomSettings(volume.profile);
        }
        
        protected override void Update()
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