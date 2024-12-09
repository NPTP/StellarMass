using System;
using UnityEngine;

namespace Summoner.Utilities.Attributes
{
    /// <summary>
    /// Use only on Object reference fields.
    /// Will keep the GUI for that field enabled in the inspector until
    /// a reference has been set, after which it will be disabled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class GUIDisabledUntilSetAttribute : PropertyAttribute
    {
    }
}
