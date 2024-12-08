using UnityEngine;

namespace Summoner.Utilities.Extensions
{
    public static class ColorExtensions
    {
        public static Color SetValues(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(
                r ?? color.r,
                g ?? color.g,
                b ?? color.b,
                a ?? color.a
            );
        }
    }
}