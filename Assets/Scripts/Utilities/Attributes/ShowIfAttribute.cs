using System;
using UnityEngine;

namespace Summoner.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string BoolName { get; }

        public ShowIfAttribute(string boolName)
        {
            BoolName = boolName;
        }
    }
}
