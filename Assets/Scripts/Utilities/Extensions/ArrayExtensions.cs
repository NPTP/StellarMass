namespace StellarMass.Utilities.Extensions
{
    public static class ArrayExtensions
    {
        public static bool HasItem<T>(this T[] array)
        {
            return array.Length > 0;
        }
    }
}