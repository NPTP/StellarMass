using FMOD.Studio;
using FMODUnity;
using Summoner.Utilities.FMODUtilities;
using Summoner.Utilities.FMODUtilities.Enums;

namespace Summoner.Systems.AudioSystem
{
    public class PersistentAudio
    {
        private EventReference currentEventReference;
        private EventInstance currentEventInstance;

        public void Play(EventReference newEventReference)
        {
            if (currentEventReference.IsSameEvent(newEventReference))
            {
                return;
            }

            currentEventInstance.Stop(StopFlags.Release);

            currentEventReference = newEventReference;
            currentEventInstance = newEventReference.CreateInstance();
            currentEventInstance.start();
        }

        public void Play()
        {
            if (currentEventReference.IsNull)
            {
                return;
            }
            
            Play(currentEventReference);
        }

        public void Stop()
        {
            currentEventInstance.Stop(StopFlags.Release);
        }
    }
}