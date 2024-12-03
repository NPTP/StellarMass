using System;
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

        public static void StopFadeOut(this EventInstance eventInstance, ReleaseOption releaseOption = ReleaseOption.None)
        {
            eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            HandleReleaseOption(ref eventInstance, releaseOption);
        }
        
        public static void StopImmediate(this EventInstance eventInstance, ReleaseOption releaseOption = ReleaseOption.None)
        {
            eventInstance.stop(STOP_MODE.IMMEDIATE);
            HandleReleaseOption(ref eventInstance, releaseOption);
        }

        private static void HandleReleaseOption(ref EventInstance eventInstance, ReleaseOption releaseOption)
        {
            switch (releaseOption)
            {
                case ReleaseOption.None:
                    break;
                case ReleaseOption.Release:
                    eventInstance.release();
                    break;
                case ReleaseOption.ReleaseAndClearHandle:
                    eventInstance.release();
                    eventInstance.clearHandle();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(releaseOption), releaseOption, null);
            }
        }
    }
}