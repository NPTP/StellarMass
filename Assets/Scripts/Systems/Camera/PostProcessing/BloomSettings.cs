using System;
using UnityEngine.Rendering.PostProcessing;

namespace StellarMass.Game.VFX.PostProcessing
{
    [Serializable]
    public class BloomSettings : EffectSettings<Bloom>
    {
        public float Diffusion
        {
            get => Setting.diffusion.value;
            set => Setting.diffusion.value = value;
        }
        
        public float DiffusionInitialValue { get; private set; }

        public BloomSettings(PostProcessProfile profile) : base(profile)
        {
            DiffusionInitialValue = Diffusion;
        }
    }
}