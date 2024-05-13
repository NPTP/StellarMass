using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace StellarMass.Utilities.FMODUtilities
{
    public static class EventInstanceExtensions
    {
        public static void AttachToTransform(this EventInstance eventInstance, Transform transform)
        {
            RuntimeManager.AttachInstanceToGameObject(eventInstance, transform);
        }

        public static void SetParameter(this EventInstance eventInstance, string parameterName, float value)
        {
            eventInstance.setParameterByName(parameterName, value);
        }
    }
}