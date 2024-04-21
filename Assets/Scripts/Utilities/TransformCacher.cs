using UnityEngine;

namespace StellarMass.Utilities
{
    public class TransformCacher : MonoBehaviour
    {
        protected new Transform transform;

        protected virtual void Awake()
        {
            transform = gameObject.transform;
        }
    }
}