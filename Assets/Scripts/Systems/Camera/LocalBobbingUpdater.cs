using Summoner.Systems.Data.Persistent;
using UnityEngine;

namespace Summoner.Systems.Camera
{
    public class LocalBobbingUpdater : MonoBehaviour
    {
        private Transform cameraTransform;

        public void Initialize(Transform camTransform)
        {
            cameraTransform = camTransform;
        }

        private void OnDisable()
        {
            cameraTransform.localPosition = Vector3.zero;
        }

        private void Update()
        {
            float time = Time.time;
            cameraTransform.localPosition = new Vector3(
                PersistentData.Camera.HorSinAmp * Mathf.Sin(PersistentData.Camera.HorSinFreq * time),
                PersistentData.Camera.VertSinAmp * Mathf.Sin(PersistentData.Camera.VertSinFreq * time), 0);
        }
    }
}