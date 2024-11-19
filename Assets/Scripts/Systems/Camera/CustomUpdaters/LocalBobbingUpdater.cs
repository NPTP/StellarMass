using Summoner.Systems.Data.Persistent;
using UnityEngine;

namespace Summoner.Systems.Camera.CustomUpdaters
{
    public class LocalBobbingUpdater : CustomUpdater
    {
        [SerializeField] private Transform cameraTransform;

        private void OnDisable()
        {
            cameraTransform.localPosition = Vector3.zero;
        }

        protected override void Update()
        {
            float time = Time.unscaledTime;
            cameraTransform.localPosition = new Vector3(
                PersistentData.Camera.HorSinAmp * Mathf.Sin(PersistentData.Camera.HorSinFreq * time),
                PersistentData.Camera.VertSinAmp * Mathf.Sin(PersistentData.Camera.VertSinFreq * time), 0);
        }
    }
}