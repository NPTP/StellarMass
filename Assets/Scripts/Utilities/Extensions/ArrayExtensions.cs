using System;

namespace StellarMass.Utilities.Extensions
{
    public static class ArrayExtensions
    {
        public static bool HasItem<T>(this T[] array)
        {
            return array != null && array.Length > 0;
        }

        public static void For<T>(this T[] array, Action<T> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action.Invoke(array[i]);
            }
        }
    }
}