using System;
using UnityEngine;

namespace Summoner.Utilities.Extensions
{
    public static class TransformExtensions
    {
        public static void ApplyToChildren(this Transform transform, Action<Transform> action)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                action(transform.GetChild(i));
            }
        }
    }
}