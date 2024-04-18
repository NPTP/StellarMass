using UnityEngine;

namespace Utilities
{
    public sealed class Singleton<T> : ClosedSingleton<T> where T : MonoBehaviour
    {
        public static T Instance => PrivateInstance;
    }
}