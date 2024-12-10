using System;
using FMODUnity;
using Summoner.Systems.Camera.CustomUpdaters;
using Summoner.Utilities;
using Summoner.Utilities.Singletons;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Summoner.Systems.Camera
{
    public class CameraController : ManualInitSingleton<CameraController>
    {
        [SerializeField] private StudioListener fmodStudioListener;
        [SerializeField] private PostProcessVolume postProcessVolume;
        [SerializeField] private CustomUpdater[] customUpdaters = Array.Empty<CustomUpdater>();

        public static PostProcessProfile PostProcessProfile { set => Instance.postProcessVolume.profile = value; }
        public static GameObject ListenerAttenuationObject { set => Instance.fmodStudioListener.SetField("attenuationObject", value); }
        
        private void OnValidate()
        {
            customUpdaters = GetComponentsInChildren<CustomUpdater>();
        }

        protected override void InitializeOverrideable()
        {
            Enable<PostProcessingUpdater>(true);
        }

        public static void EnablePostProcessVolume(bool enable) => Instance.postProcessVolume.enabled = enable;
        
        public static void Enable<TCustomUpdater>(bool enable) where TCustomUpdater : CustomUpdater =>
            Instance.EnablePrivate<TCustomUpdater>(enable);

        private void EnablePrivate<TCustomUpdater>(bool enable) where TCustomUpdater : CustomUpdater
        {
            foreach (CustomUpdater customUpdater in customUpdaters)
            {
                if (customUpdater.GetType() == typeof(TCustomUpdater))
                {
                    customUpdater.enabled = enable;
                    return;
                }
            }

            Debug.Log($"No {nameof(CustomUpdater)} of type {typeof(TCustomUpdater).Name} found on {nameof(CameraController)}.");
        }
    }
}