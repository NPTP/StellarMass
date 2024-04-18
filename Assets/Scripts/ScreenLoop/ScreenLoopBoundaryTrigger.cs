using System;
using StellarMass.Utilities.Attributes;
using StellarMass.Utilities.Extensions;
using UnityEngine;

namespace StellarMass.ScreenLoop
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ScreenLoopBoundaryTrigger : MonoBehaviour
    {
        [SerializeField] [Required] private BoxCollider2D boxCollider2D;
        
        public event Action<Collider2D> OnEntered;
        public event Action<Collider2D> OnExited;

        public Bounds Bounds => boxCollider2D.bounds;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.IsLoopableCollider())
            {
                OnEntered?.Invoke(col);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.IsLoopableCollider())
            {
                OnExited?.Invoke(col);
            }
        }
    }
}