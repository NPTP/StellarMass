using System.Collections.Generic;
using StellarMass.Utilities.Attributes;
using UnityEngine;

namespace StellarMass.ScreenLoop
{
    [RequireComponent(typeof(Collider2D))]
    public class LoopableCollider : MonoBehaviour
    {
        private static readonly HashSet<Collider2D> loopableColliders = new();
        public static bool IsLoopableCollider(Collider2D collider2D) => loopableColliders.Contains(collider2D);

        [SerializeField][Required] private Collider2D loopableCollider;

        private void OnEnable()
        {
            loopableColliders.Add(loopableCollider);
        }

        private void OnDisable()
        {
            loopableColliders.Remove(loopableCollider);
        }
    }
}