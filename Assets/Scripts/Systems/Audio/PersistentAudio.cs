using FMOD.Studio;
using FMODUnity;
using StellarMass.Utilities.FMODUtilities;

namespace StellarMass.Systems.AudioSystem
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

            currentEventInstance.StopFadeOut(release: true);

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
            currentEventInstance.StopFadeOut(release: true);
        }
    }
}