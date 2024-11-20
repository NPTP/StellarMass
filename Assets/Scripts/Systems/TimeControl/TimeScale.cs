using System.Collections.Generic;
using UnityEngine;

namespace Summoner.Systems.TimeControl
{
    /// <summary>
    /// Handle all time scale changing requests, prioritizing the most recent requests over all others.
    /// </summary>
    public static class TimeScale
    {
        private const float DEFAULT_TIME_SCALE = 1.0f;

        private class TimeScaleSetter
        {
            public ITimescaleChanger TimescaleChanger { get; }
            public float timeScale;
            public TimeScaleSetter(ITimescaleChanger timescaleChanger) => TimescaleChanger = timescaleChanger;
            public void ApplyTimeScale() => Time.timeScale = timeScale;
        }

        private static bool initialized;
        private static readonly List<TimeScaleSetter> timeScaleSetters = new();

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            timeScaleSetters.Clear();
            ResetTimeScaleToDefault();
        }

        public static void Change(ITimescaleChanger timescaleChanger, float timeScale)
        {
            TimeScaleSetter timeScaleSetter;
            if (TryGetIndexOfTimeScaleChanger(timescaleChanger, out int index))
            {
                timeScaleSetter = timeScaleSetters[index];
                timeScaleSetters.RemoveAt(index);
            }
            else
            {
                timeScaleSetter = new TimeScaleSetter(timescaleChanger);
            }
            
            timeScaleSetter.timeScale = timeScale;
            timeScaleSetter.ApplyTimeScale();
            timeScaleSetters.Add(timeScaleSetter);
        }

        public static void Reset(ITimescaleChanger timescaleChanger)
        {
            if (TryGetIndexOfTimeScaleChanger(timescaleChanger, out int index))
            {
                timeScaleSetters.RemoveAt(index);
            }
            
            if (timeScaleSetters.Count > 0)
                timeScaleSetters[^1].ApplyTimeScale();
            else
                ResetTimeScaleToDefault();
        }

        private static void ResetTimeScaleToDefault()
        {
            Time.timeScale = DEFAULT_TIME_SCALE;
        }

        private static bool TryGetIndexOfTimeScaleChanger(ITimescaleChanger timescaleChanger, out int index)
        {
            for (int i = 0; i < timeScaleSetters.Count; i++)
            {
                TimeScaleSetter timeScaleSetter = timeScaleSetters[i];
                if (timeScaleSetter.TimescaleChanger == timescaleChanger)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }
    }
}
