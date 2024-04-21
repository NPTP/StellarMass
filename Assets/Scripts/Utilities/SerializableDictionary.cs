using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Based on one of Unity's serializable dictionaries for a customizable/maintainable version
    /// </summary>
    [Serializable]
    public sealed class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys;
        [SerializeField]
        private List<TValue> values;
        private Dictionary<TKey, TValue> internalDictionary = new();

        public ICollection<TKey> Keys => internalDictionary.Keys;
        public ICollection<TValue> Values => internalDictionary.Values;
        public int Count => internalDictionary.Count;
        
        public TValue this[TKey key]
        {
            get => internalDictionary[key];
            set => internalDictionary[key] = value;
        }

        public TKey this[TValue value]
        {
            get
            {
                keys = new List<TKey>(internalDictionary.Keys);
                values = new List<TValue>(internalDictionary.Values);
                int index = values.FindIndex(x => x.Equals(value));
                if (index < 0)
                {
                    throw new KeyNotFoundException();
                }
                return keys[index];
            }
        }

        public void Add(TKey key, TValue value) => internalDictionary.Add(key, value);
        public bool ContainsKey(TKey key) => internalDictionary.ContainsKey(key);
        public bool Remove(TKey key) => internalDictionary.Remove(key);
        public bool TryGetValue(TKey key, out TValue value) => internalDictionary.TryGetValue(key, out value);
        public void Clear() => internalDictionary.Clear();
        public IEnumerator GetEnumerator() => internalDictionary.GetEnumerator();

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(internalDictionary.Keys);
            values = new List<TValue>(internalDictionary.Values);
        }

        public void OnAfterDeserialize()
        {
            Debug.Assert(keys.Count == values.Count);
            Clear();
            for (int i = 0; i < keys.Count; ++i)
            {
                Add(keys[i], values[i]);
            }
        }
        
        #region Explicit IDictionary Implementations
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => (internalDictionary as ICollection<KeyValuePair<TKey, TValue>>).IsReadOnly;
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => (internalDictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => (internalDictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => (internalDictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => (internalDictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => (internalDictionary as IEnumerable<KeyValuePair<TKey, TValue>>).GetEnumerator();
        #endregion
    }
}