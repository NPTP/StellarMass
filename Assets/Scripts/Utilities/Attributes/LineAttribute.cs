using System;
using UnityEngine;

namespace Summoner.Utilities.Attributes
{
    /// <summary>
    /// Use this attribute to draw a line in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class LineAttribute : PropertyAttribute
    {
    }
}