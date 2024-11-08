using System.Collections;
using StellarMass.Utilities;
using StellarMass.Utilities.Singletons;
using UnityEngine;

namespace StellarMass.Game.Cameras
{
    public class CameraController : ManualInitSingleton<CameraController>
    {
        [SerializeField] private Transform cameraTransform;

        [SerializeField] private float vertSinAmp = 1;
        [SerializeField] private float vertSinFreq = 1;

        [SerializeField] private float horSinAmp = 1;
        [SerializeField] private float horSinFreq = 1;

        private Coroutine localBobCoroutine;

        private void OnDisable()
        {
            StopMoving();
        }

        public static void StartMoving()
        {
            CoroutineUtility.StopAndReplaceCoroutine(Instance, ref Instance.localBobCoroutine, Instance.LocalBobRoutine());
        }

        public static void StopMoving()
        {
            CoroutineUtility.StopAndNullCoroutine(Instance, ref Instance.localBobCoroutine);
            Instance.cameraTransform.localPosition = Vector3.zero;
        }

        private IEnumerator LocalBobRoutine()
        {
            while (true)
            {
                float time = Time.time;
                Instance.cameraTransform.localPosition = new Vector3(
                    Instance.horSinAmp * Mathf.Sin(Instance.horSinFreq * time),
                    Instance.vertSinAmp * Mathf.Sin(Instance.vertSinFreq * time), 0);
                yield return null;
            }
        }
    }
}