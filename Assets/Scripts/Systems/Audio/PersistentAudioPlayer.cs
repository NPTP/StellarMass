using System.Collections.Generic;
using FMODUnity;

namespace StellarMass.Systems.AudioSystem
{
    public static class PersistentAudioPlayer
    {
        private static readonly List<PersistentAudio> persistentAudios = new List<PersistentAudio>();

        public static PersistentAudio PlayPersistentAudio(EventReference eventReference)
        {
            PersistentAudio persistentAudio = new PersistentAudio();
            persistentAudio.Play(eventReference);
            persistentAudios.Add(persistentAudio);
            return persistentAudio;
        }

        public static void StopAll()
        {
            foreach (PersistentAudio persistentAudio in persistentAudios)
            {
                persistentAudio.Stop();
            }
        }
    }
}