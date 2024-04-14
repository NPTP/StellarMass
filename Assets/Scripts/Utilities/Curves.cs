namespace StellarMass.Utilities
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
    }
}