using System;
using StellarMass.Ship;
using StellarMass.Utilities.Attributes;
using UnityEngine;

namespace StellarMass.ScreenLoop
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class LoopCollider : MonoBehaviour
    {
        [SerializeField] [Required] private BoxCollider2D boxCollider2D;
        
        public event Action<Collider2D> OnEntered;
        public event Action<Collider2D> OnExited;

        public Bounds Bounds => boxCollider2D.bounds;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.IsPlayer())
            {
                OnEntered?.Invoke(col);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.IsPlayer())
            {
                OnExited?.Invoke(col);
            }
        }
    }
}