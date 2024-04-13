using System;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StellarMass.Audio
{
    [CreateAssetMenu]
    [Serializable]
    public class Sound : ScriptableObject
    {
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private PlaybackType playbackType;
        
        [SerializeField][Range(0, 1)] private float volume = 1;
        public float Volume => volume;
        
        [SerializeField] private bool loop;
        public bool Loop => loop;
        
        [SerializeField] private bool is3D;
        public bool Is3D => is3D;

        private int rrIndex;
        
        public AudioClip GetClip()
        {
            if (clips == null || !clips.HasItem())
            {
                return null;
            }

            AudioClip clip;
            
            switch (playbackType)
            {
                case PlaybackType.Single:
                    clip = clips[0];
                    break;
                case PlaybackType.Random:
                    int randomIndex = Random.Range(0, clips.Length - 1);
                    clip = clips[randomIndex];
                    clips[randomIndex] = clips[^1];
                    clips[^1] = clip;
                    break;
                case PlaybackType.RoundRobin:
                    clip = clips[rrIndex];
                    rrIndex = (rrIndex + 1) % clips.Length;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return clip;
        }
    }
}