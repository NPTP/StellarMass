using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace StellarMass.Utilities.FMODUtilities
{
    public static class EventReferenceExtensions
    {
        public static void PlayOneShot(this EventReference eventReference, Vector3 position = new())
        {
            RuntimeManager.PlayOneShot(eventReference, position);
        }

        public static void PlayOneShot(this EventReference eventReference, Transform transform)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.AttachToTransform(transform);
            eventInstance.start();
            eventInstance.release();
        }
    }
}