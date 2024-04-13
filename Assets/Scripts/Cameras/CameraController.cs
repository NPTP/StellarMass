using UnityEngine;

namespace StellarMass.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;

        [SerializeField] private float vertSinAmp = 1;
        [SerializeField] private float vertSinFreq = 1;
        
        [SerializeField] private float horSinAmp = 1;
        [SerializeField] private float horSinFreq = 1;

        private void Update()
        {
            float time = Time.time;
            cameraTransform.localPosition = new Vector3(
                horSinAmp * Mathf.Sin(horSinFreq * time),
                vertSinAmp * Mathf.Sin(vertSinFreq * time),
                0);
        }
    }
}