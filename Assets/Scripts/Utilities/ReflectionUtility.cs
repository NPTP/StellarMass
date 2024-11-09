using System;
using System.Reflection;
using UnityEngine;

namespace Summoner.Utilities
{
    public static class ReflectionUtility
    {
        public static T GetField<T>(this object obj, string fieldName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentNullException(nameof(fieldName));

            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, bindingFlags);

            if (fieldInfo == null)
            {
                Debug.LogError($"Field '{fieldName}' not found in {type.FullName} with the specified binding flags.");
                return default;
            }

            object value = fieldInfo.GetValue(obj);
            if (value is not T tValue)
            {
                Debug.LogError(value == null
                    ? $"Field '{fieldName}' is null."
                    : $"Field '{fieldName}' is not of type {typeof(T)}.");

                return default;
            }

            return tValue;
        }
        
        public static bool SetField<T>(this object obj, string fieldName, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentNullException(nameof(fieldName));

            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, bindingFlags);

            if (fieldInfo == null)
            {
                Debug.LogError($"Field '{fieldName}' not found in {type.FullName} with the specified binding flags.");
                return false;
            }

            if (!fieldInfo.FieldType.IsAssignableFrom(typeof(T)))
            {
                Debug.LogError($"Value type '{typeof(T)}' is not assignable to field type '{fieldInfo.FieldType}' in field '{fieldName}' of {type.FullName}.");
                return false;
            }

            fieldInfo.SetValue(obj, value);
            return true;
        }
    }
}