using System;
using FMOD.Studio;
using Summoner.Game.SaveAndLoad;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.SaveAndLoad;
using UnityEngine;

namespace Summoner.Systems.AudioSystem
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

            InitializeFmodBuses(PD.Audio.MasterBusPath, PD.Audio.MusicBusPath, PD.Audio.SfxBusPath);

            initialized = true;
        }
        
        private static void InitializeFmodBuses(string masterBusPath, string musicBusPath, string sfxBusPath)
        {
            masterBus = FMODUnity.RuntimeManager.GetBus(masterBusPath);
            musicBus = FMODUnity.RuntimeManager.GetBus(musicBusPath);
            sfxBus = FMODUnity.RuntimeManager.GetBus(sfxBusPath);

            SaveLoad.Get(out GameSettings settings);
            SetMasterVolume(settings.masterVolume);
            SetMusicVolume(settings.musicVolume);
            SetSfxVolume(settings.sfxVolume);
        }

        private static void SetMasterVolume(float linearInput)
        {
            SetBusVolume(linearInput, masterBus, vol => SaveLoad.Get<GameSettings>().masterVolume = vol);
        }
        
        private static void SetMusicVolume(float linearInput)
        {
            SetBusVolume(linearInput, musicBus, vol => SaveLoad.Get<GameSettings>().musicVolume = vol);
        }
        
        private static void SetSfxVolume(float linearInput)
        {
            SetBusVolume(linearInput, sfxBus, vol => SaveLoad.Get<GameSettings>().sfxVolume = vol);
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