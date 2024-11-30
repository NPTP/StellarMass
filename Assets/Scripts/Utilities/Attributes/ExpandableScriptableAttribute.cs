using UnityEngine;

namespace Summoner.Utilities.Attributes
{
    /// <summary>
    /// Attribute for ScriptableObjects to let their contents be expanded, shown and edited in the inspector
    /// of another object.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ExpandableScriptableAttribute : PropertyAttribute
    {
    }
}