using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Summoner.Systems.AssetLoading
{
    public class Asset<T> where T : Object
    {
        public T Value { get; }

        private AsyncOperationHandle<T> Handle { get; }

        public Asset(AsyncOperationHandle<T> handle)
        {
            Value = handle.Result;
            Handle = handle;
        }

        public void Release()
        {
            Addressables.Release(Handle);
        }
    }
}
