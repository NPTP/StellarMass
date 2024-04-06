using System;
using UnityEngine.Rendering.PostProcessing;

namespace StellarMass.VFX
{
    public abstract class EffectSettings { }

    public abstract class EffectSettings<T> where T : PostProcessEffectSettings { }

    [Serializable]
    public class BloomSettings : EffectSettings<Bloom>
    {
        private Bloom bloom;

        public float Diffusion
        {
            get => bloom.diffusion.value;
            set => bloom.diffusion.value = value;
        }
        public float DiffusionInitialValue { get; private set; }

        public BloomSettings(PostProcessProfile profile)
        {
            profile.TryGetSettings(out bloom);
            DiffusionInitialValue = Diffusion;
        }
    }
}