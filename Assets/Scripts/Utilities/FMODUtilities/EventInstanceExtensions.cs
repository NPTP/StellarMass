using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

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
        
        public static bool IsOneShot(this EventInstance eventInstance)
        {
            eventInstance.getDescription(out EventDescription description);
            description.isOneshot(out bool isOneShot);
            return isOneShot;
        }
        
        public static bool IsSameEvent(this EventInstance eventInstance, EventReference eventReference)
        {
            if (!eventInstance.isValid() || eventReference.IsNull) return false;
            
            eventInstance.getDescription(out EventDescription eventInstanceDescription);
            eventInstanceDescription.getID(out GUID eventInstanceGuid);

            return eventInstanceGuid == eventReference.Guid;
        }

        public static void StopFadeOut(this EventInstance eventInstance, bool release = false)
        {
            eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            if (release) eventInstance.release();
        }
        
        public static void StopImmediate(this EventInstance eventInstance, bool release = false)
        {
            eventInstance.stop(STOP_MODE.IMMEDIATE);
            if (release) eventInstance.release();
        }
    }
}