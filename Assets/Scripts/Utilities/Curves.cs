using UnityEngine;

namespace Summoner.Utilities
{
    public static class Curves
    {
        public static float Square(float x)
        {
            if (x >= 1) return 1;
            return x * x;
        }

        public static float SmoothStep(float x)
        {
            if (x >= 1) return 1;
            return x * x * (3.0f - 2.0f * x);
        }

        public static float EaseOutQuart(float x)
        {
            return EaseOutToPower(x, 4);
        }

        public static float EaseOutExp(float x)
        {
            return x >= 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
        }

        private static float EaseOutToPower(float x, float p)
        {
            return 1 - Mathf.Pow(1 - x, p);
        }
    }
}