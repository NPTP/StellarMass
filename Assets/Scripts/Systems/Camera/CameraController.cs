using System;
using FMODUnity;
using Summoner.Systems.Camera.CustomUpdaters;
using Summoner.Utilities;
using Summoner.Utilities.Singletons;
using UnityEngine;

namespace Summoner.Systems.Camera
{
    public class CameraController : ManualInitSingleton<CameraController>
    {
        [SerializeField] private StudioListener fmodStudioListener;
        [SerializeField] private CustomUpdater[] customUpdaters = Array.Empty<CustomUpdater>();

        private void OnValidate()
        {
            customUpdaters = GetComponentsInChildren<CustomUpdater>();
        }

        protected override void InitializeOverrideable()
        {
            Enable<PostProcessingUpdater>(true);
        }

        public void SetListenerAttenuationObject(GameObject go)
        {
            fmodStudioListener.SetField("attenuationObject", go);
        }

        public void Enable<TCustomUpdater>(bool enable) where TCustomUpdater : CustomUpdater
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