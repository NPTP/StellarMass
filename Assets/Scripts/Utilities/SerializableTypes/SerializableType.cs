using System;
using UnityEngine;

namespace StellarMass.Utilities.SerializableTypes
{
    [Serializable]
    public class SerializableType
    {
        [SerializeField] private string typeName;

        private Type runtimeType;
        
        public Type Value => runtimeType ??= Type.GetType(typeName);
    }
}