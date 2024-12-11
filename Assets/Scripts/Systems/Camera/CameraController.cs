using FMODUnity;
using Summoner.Systems.Camera.CameraComponents;
using Summoner.Utilities;
using Summoner.Utilities.Singletons;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Summoner.Systems.Camera
{
    public class CameraController : ManualInitSingleton<CameraController>
    {
        [SerializeField] private Transform cameraTransform;
        public Transform CameraTransform => cameraTransform;
        
        [SerializeField] private PostProcessVolume postProcessVolume;
        public PostProcessVolume PostProcessVolume => postProcessVolume;

        [SerializeField] private StudioListener fmodStudioListener;
        
        protected override void InitializeOverrideable()
        {
            Enable<PostProcessingUpdater>(true);
        }

        public void SetListenerAttenuationObject(GameObject go)
        {
            fmodStudioListener.SetField("attenuationObject", go);
        }

        public static void Enable<TCustomUpdater>(bool enable) where TCustomUpdater : CameraExtension => Instance.EnablePrivate<TCustomUpdater>(enable);
        private void EnablePrivate<TCustomUpdater>(bool enable) where TCustomUpdater : CameraExtension
        {
            if (enable)
                CameraExtension.Create<TCustomUpdater>(this);
            else
                CameraExtension.Remove<TCustomUpdater>(this);
        }
    }
}