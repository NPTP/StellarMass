using FMODUnity;
using Summoner.Utilities;
using Summoner.Utilities.Singletons;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Summoner.Systems.Camera
{
    public class CameraController : ManualInitSingleton<CameraController>
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private PostProcessVolume volume;
        [SerializeField] private StudioListener fmodStudioListener;
        
        [Header("Enableable/Disableable")]
        [SerializeField] private LocalBobbingUpdater localBobbingUpdater;
        [SerializeField] private PostProcessingUpdater postProcessingUpdater;

        protected override void InitializeOverrideable()
        {
            postProcessingUpdater.Initialize(volume);
            localBobbingUpdater.Initialize(cameraTransform);
            
            postProcessingUpdater.enabled = true;
        }

        public void SetListenerAttenuationObject(GameObject go)
        {
            fmodStudioListener.SetField("attenuationObject", go);
        }
    }
}