using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StellarMass.Utilities;
using UnityEngine;
using Utilities;

namespace StellarMass.Audio
{
    public class SoundPlayer : Singleton<SoundPlayer>
    {
        [SerializeField] private int audioSourcePoolSize = 32;
        [SerializeField] private Sound[] startingSounds;

        private readonly Dictionary<Sound, List<AudioSource>> soundToAudioSources = new();
        private AudioSource[] pool;

        private void Start()
        {
            InitializePool();
            
            foreach (Sound sound in startingSounds)
            {
                PlaySound(sound);
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

        public static void StopAll(Sound sound) => Instance.StopAllInternal(sound);
        private void StopAllInternal(Sound sound)
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

        public static void PlaySound(Sound sound) => Instance.PlaySoundInternal(sound);
        private void PlaySoundInternal(Sound sound)
        {
            StartCoroutine(PlaySoundRoutine(sound));
        }

        private IEnumerator PlaySoundRoutine(Sound sound)
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

            float destinationVolume = sound.Volume;
            inactiveAudioSource.volume = sound.FadeInOnPlay ? 0 : destinationVolume;
            
            inactiveAudioSource.Play();

            float elapsed = 0;
            while (inactiveAudioSource.isPlaying)
            {
                if (sound.FadeInOnPlay && inactiveAudioSource.volume < destinationVolume)
                {
                    inactiveAudioSource.volume = Mathf.Lerp(0, destinationVolume, Curves.SmoothStep(elapsed / sound.FadeInTime));
                }
                
                yield return null;
                
                elapsed += Time.unscaledDeltaTime;
            }

            audioSources.Remove(inactiveAudioSource);
            if (audioSources.Count == 0)
            {
                soundToAudioSources.Remove(sound);
            }
        }
    }
}