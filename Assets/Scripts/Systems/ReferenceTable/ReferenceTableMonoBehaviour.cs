using UnityEngine;

namespace Summoner.Systems.ReferenceTable
{
    public class ReferenceTableMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake() => MonoReferenceTable.TryAdd(this);
        protected virtual void OnDestroy() => MonoReferenceTable.Remove(this);
    }
}