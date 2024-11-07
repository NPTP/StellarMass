using UnityEngine;

namespace StellarMass.Systems.ReferenceTable
{
    public class ReferenceableMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake() => MonoReferenceTable.TryAdd(this);
        protected virtual void OnDestroy() => MonoReferenceTable.Remove(this);
    }
}