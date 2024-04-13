using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace StellarMass.Audio
{
    public class SoundPlayer : Singleton<SoundPlayer>
    {
        [SerializeField] private int audioSourcePoolSize = 32;
        [SerializeField] private Sound[] startingSounds;

        private Dictionary<Sound, List<AudioSource>> soundToAudioSources = new();
        private AudioSource[] pool;

        private void Start()
        {
            InitializePool();
            
            foreach (Sound sound in startingSounds)
            {
                PlaySound2D(sound);
            }
        }

        private void InitializePool()
        {
            pool = new AudioSource[audioSourcePoolSize];
            transform.SetParent(this.transform);
            transform.localPosition = Vector3.zero;
            
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                GameObject g = new GameObject($"AudioSource [{i + 1:00}]");
                g.transform.SetParent(transform);
                g.transform.localPosition = Vector3.zero;
                AudioSource a = g.AddComponent<AudioSource>();
                a.playOnAwake = false;
                pool[i] = a;
            }
        }

        public static void StopSound(Sound sound) => Instance.StopSoundInternal(sound);
        private void StopSoundInternal(Sound sound)
        {
            if (!soundToAudioSources.TryGetValue(sound, out List<AudioSource> audioSources))
            {
                return;
            }

            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Stop();
            }
        }

        public static void PlaySound2D(Sound sound) => Instance.PlaySound2DInternal(sound);
        private void PlaySound2DInternal(Sound sound)
        {
            if (sound.Is3D)
            {
                return;
            }

            StartCoroutine(PlaySound(sound));
        }

        private IEnumerator PlaySound(Sound sound)
        {
            AudioClip clip = sound.GetClip();
            if (clip == null)
            {
                yield break;
            }

            AudioSource inactiveAudioSource = pool.FirstOrDefault(a => !a.isPlaying);
            if (inactiveAudioSource == null)
            {
                yield break;
            }

            if (soundToAudioSources.TryGetValue(sound, out List<AudioSource> audioSources))
            {
                audioSources.Add(inactiveAudioSource);
            }
            else
            {
                audioSources = new List<AudioSource>() { inactiveAudioSource };
                soundToAudioSources.Add(sound, audioSources);
            }
            
            inactiveAudioSource.spatialize = sound.Is3D;
            inactiveAudioSource.clip = clip;
            inactiveAudioSource.loop = sound.Loop;
            inactiveAudioSource.volume = sound.Volume;
            inactiveAudioSource.Play();

            while (inactiveAudioSource.isPlaying)
            {
                yield return null;
            }

            audioSources.Remove(inactiveAudioSource);
            if (audioSources.Count == 0)
            {
                soundToAudioSources.Remove(sound);
            }
        }
    }
}