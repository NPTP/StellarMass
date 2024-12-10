using UnityEngine;

namespace Summoner.Utilities.CurveUtilities
{
    /// <summary>
    /// Process a value x in the interval [0, 1] to get the curved value.
    /// </summary>
    public static class Curves
    {
        public static float Quad(float x)
        {
            if (x >= 1) return 1;
            return x * x;
        }

        public static float SmoothStep(float x)
        {
            if (x >= 1) return 1;
            return x * x * (3.0f - 2.0f * x);
        }

        public static float EaseOutExp(float x)
        {
            return x >= 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
        }

        private static float OutQuad(float x)
        {
            return -x * (x - 2.0f);
        }

        public static float EaseOutQuad(float x) => EaseOutToPower(x, 2);
        public static float EaseOutCubic(float x) => EaseOutToPower(x, 3);
        public static float EaseOutQuart(float x) => EaseOutToPower(x, 4);

        private static float EaseOutToPower(float x, float p)
        {
            return 1 - Mathf.Pow(1 - x, p);
        }
    }
}