using UnityEngine.Rendering.PostProcessing;

namespace StellarMass.Game.VFX.PostProcessing
{
    public abstract class EffectSettings { }

    public abstract class EffectSettings<T> where T : PostProcessEffectSettings
    {
        protected T Setting { get; }

        public EffectSettings(PostProcessProfile profile)
        {
            Setting = profile.GetSetting<T>();
        }
    }
}