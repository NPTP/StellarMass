using System;
using UnityEngine;

namespace Summoner.Utilities.SerializableTypes
{
    [Serializable]
    public abstract class SerializableType
    {
        [SerializeField] private string assemblyQualifiedName;

        private Type runtimeType;
        public Type Value => runtimeType ??= Type.GetType(assemblyQualifiedName);
    }
    
    [Serializable]
    public class SerializableType<T> : SerializableType { }
}