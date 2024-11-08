using FMODUnity;
using StellarMass.Utilities;
using StellarMass.Utilities.Singletons;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace StellarMass.Systems.Camera
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
            postProcessingUpdater.enabled = true;
            
            localBobbingUpdater.Initialize(cameraTransform);
            localBobbingUpdater.enabled = true;
            
            SetListenerAttenuationObject();
        }

        public void SetListenerAttenuationObject()
        {
            fmodStudioListener.SetField("attenuationObject", gameObject);
        }
    }
}