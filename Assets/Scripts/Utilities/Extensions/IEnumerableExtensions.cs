using System;
using System.Collections.Generic;

namespace Summoner.Utilities.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> iEnumerable, Action<T> action)
        {
            if (iEnumerable == null || action == null)
            {
                return;
            }
            
            foreach (T value in iEnumerable)
            {
                action.Invoke(value);
            }
        }
    }
}