using System;
using FMOD.Studio;
using StellarMass.Systems.Data.Persistent;
using StellarMass.Systems.SaveAndLoad;
using UnityEngine;

namespace StellarMass.Systems.AudioSystem
{
    public static class Audio
    {
        private static Bus masterBus;
        private static Bus musicBus;
        private static Bus sfxBus;
        
        private static bool initialized;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            InitializeFmodBuses();

            initialized = true;
        }
        
        private static void InitializeFmodBuses()
        {
            masterBus = FMODUnity.RuntimeManager.GetBus(PersistentData.Core.MasterBusPath);
            musicBus = FMODUnity.RuntimeManager.GetBus(PersistentData.Core.MusicBusPath);
            sfxBus = FMODUnity.RuntimeManager.GetBus(PersistentData.Core.SfxBusPath);

            SaveLoad.Get(out AudioBusSettings settings);
            SetMasterVolume(settings.masterVolume);
            SetMusicVolume(settings.musicVolume);
            SetSfxVolume(settings.sfxVolume);
        }

        private static void SetMasterVolume(float linearInput)
        {
            SetBusVolume(linearInput, masterBus, vol => SaveLoad.Get<AudioBusSettings>().masterVolume = vol);
        }
        
        private static void SetMusicVolume(float linearInput)
        {
            SetBusVolume(linearInput, musicBus, vol => SaveLoad.Get<AudioBusSettings>().musicVolume = vol);
        }
        
        private static void SetSfxVolume(float linearInput)
        {
            SetBusVolume(linearInput, sfxBus, vol => SaveLoad.Get<AudioBusSettings>().sfxVolume = vol);
        }

        private static void SetBusVolume(float linearInput, Bus bus, Action<float> settingsSetter)
        {
            bus.setVolume(GetDBCurveValue(linearInput));
            settingsSetter.Invoke(linearInput);
        }

        /// <summary>
        /// Convert a linear [0, 1] input into a more accurate dB curve for the FMOD audio buses.
        /// </summary>
        private static float GetDBCurveValue(float linearInput)
        {
            return Mathf.Clamp01(linearInput * Mathf.Sqrt(linearInput));
        }
    }
}