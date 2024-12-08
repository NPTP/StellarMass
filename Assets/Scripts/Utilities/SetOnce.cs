using System;

namespace Summoner.Utilities
{
    /// <summary>
    /// Workaround for readonly fields for MonoBehaviour fields which don't have constructors.
    /// </summary>
    public struct SetOnce<T>
    {
        private bool hasBeenSet;

        private T value;
        public T Value
        {
            get => value;
            private set
            {
                if (hasBeenSet)
                {
                    throw new Exception($"SetOnce<{typeof(T)}> can only be set once, trying to set it again!");
                }
                
                this.value = value;
                hasBeenSet = true;
            }
        }
        
        private SetOnce(T value) : this()
        {
            Value = value;
        }

        public static implicit operator SetOnce<T>(T value) => new(value);
        public static implicit operator T(SetOnce<T> setOnce) => setOnce.Value;
    }
}