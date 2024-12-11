using Summoner.Game.VFX.PostProcessing;
using Summoner.Systems.Data.Persistent;
using UnityEngine;

namespace Summoner.Systems.Camera.CameraComponents
{
    public sealed class PostProcessingUpdater : CameraComponent
    {
        private BloomSettings bloomSettings;

        protected override void OnCreated(CameraController cameraController)
        {
            bloomSettings = new BloomSettings(cameraController.PostProcessVolume.profile);
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