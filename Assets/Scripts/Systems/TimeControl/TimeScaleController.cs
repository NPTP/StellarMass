using System.Collections.Generic;
using UnityEngine;

namespace Summoner.Systems.TimeControl
{
    public static class TimeScaleController
    {
        private static Queue<ITimescaleChanger> timescaleChangers = new();

        public static void RequestTimeScaleChange(ITimescaleChanger timescaleChanger, float timeScale)
        {
            // TODO: stack'em, layer'em, queue'em, whatever. For now just set it directly.
            Time.timeScale = timeScale;
        }

        public static void ResetTimeScaleChanger(ITimescaleChanger timescaleChanger)
        {
            // TODO: Drops down to the previous one
            Time.timeScale = 1;
        }
    }
}
