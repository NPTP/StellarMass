using System;
using UnityEngine;

namespace Summoner.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : PropertyAttribute { }
}