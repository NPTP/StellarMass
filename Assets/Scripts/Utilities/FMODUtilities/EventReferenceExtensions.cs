using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Summoner.Utilities.FMODUtilities
{
    public static class EventReferenceExtensions
    {
        public static EventInstance PlayOneShot(this EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.start();
            eventInstance.release();
            return eventInstance;
        }
        
        public static EventInstance PlayOneShot(this EventReference eventReference, Vector3 position)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.set3DAttributes(position.To3DAttributes());
            eventInstance.start();
            eventInstance.release();
            return eventInstance;
        }

        public static EventInstance PlayOneShot(this EventReference eventReference, Transform transform)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.AttachToTransform(transform);
            eventInstance.start();
            eventInstance.release();
            return eventInstance;
        }
        
        public static EventInstance CreateInstance(this EventReference eventReference)
        {
            EventDescription eventDesc = GetEventDescription(eventReference);
            eventDesc.createInstance(out EventInstance newInstance);
            return newInstance;
        }

        public static bool IsOneShot(this EventReference eventReference)
        {
            EventDescription eventDescription = GetEventDescription(eventReference);
            eventDescription.isOneshot(out bool isOneShot);
            return isOneShot;
        }
        
        /// <summary>
        /// This wrapper prevents exceptions from being thrown. We prefer audio code to fail silently! (Pun intended)
        /// </summary>
        private static EventDescription GetEventDescription(this EventReference eventReference)
        {
            try
            {
                return RuntimeManager.GetEventDescription(eventReference.Guid);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                return new EventDescription();
            }
        }
        
        public static bool IsSameEvent(this EventReference eventReference, EventInstance eventInstance)
        {
            if (!eventInstance.isValid() || eventReference.IsNull) return false;
            
            eventInstance.getDescription(out EventDescription eventInstanceDescription);
            eventInstanceDescription.getID(out GUID eventInstanceGuid);

            return eventInstanceGuid == eventReference.Guid;
        }
        
        public static bool IsSameEvent(this EventReference eventReference, EventReference otherReference)
        {
            if (otherReference.IsNull || eventReference.IsNull) return false;
            return otherReference.Guid == eventReference.Guid;
        }
    }
}