using System.Collections;
using UnityEngine;
using Utilities;

namespace StellarMass.Audio
{
    public class SoundPlayer : Singleton<SoundPlayer>
    {
        [SerializeField] private Sound music;

        private void Start()
        {
            PlaySound2D(music);
        }

        public static void PlaySound2D(Sound sound) => Instance.PlaySound2DInternal(sound);
        private void PlaySound2DInternal(Sound sound)
        {
            if (sound.Is3D)
            {
                return;
            }

            StartCoroutine(PlayAndRemoveAudioSource(sound));
        }

        private IEnumerator PlayAndRemoveAudioSource(Sound sound)
        {
            AudioClip clip = sound.GetClip();
            if (clip == null)
            {
                yield break;
            }

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialize = sound.Is3D;
            audioSource.clip = clip;
            audioSource.loop = sound.Loop;
            audioSource.Play();
            
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            
            Destroy(audioSource);
        }
    }
}