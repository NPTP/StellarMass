using UnityEngine;

namespace Summoner.Systems.MonoReferences
{
    public abstract class ReferenceTableMonoBehaviour : MonoBehaviour { }
    
    public abstract class ReferenceTableMonoBehaviour<T> : ReferenceTableMonoBehaviour where T : ReferenceTableMonoBehaviour
    {
        public static bool TryGet(out T reference) => MonoReferenceTable.TryGet(out reference);
        
        protected virtual void Awake() => MonoReferenceTable.TryAdd(this);
        protected virtual void OnDestroy() => MonoReferenceTable.Remove(this);
    }
}