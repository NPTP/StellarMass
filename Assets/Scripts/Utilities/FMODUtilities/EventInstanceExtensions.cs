using System.Runtime.CompilerServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Summoner.Utilities.FMODUtilities.Enums;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Summoner.Utilities.FMODUtilities
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

        public static void Stop(this EventInstance eventInstance, StopFlags stopFlags = 0)
        {
            eventInstance.stop(hasFlag(StopFlags.Immediate) ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);

            if (hasFlag(StopFlags.Release))
                eventInstance.release();

            if (hasFlag(StopFlags.ClearHandle))
                eventInstance.clearHandle();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool hasFlag(StopFlags other) => (stopFlags & other) != 0;
        }
    }
}