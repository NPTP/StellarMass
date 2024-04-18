using StellarMass.Ship;
using UnityEngine;

namespace StellarMass.Utilities.Extensions
{
    public static class Collider2DExtensions
    {
        public static bool IsPlayer(this Collider2D collider2D)
        {
            return collider2D == ShipMovement.PlayerCollider2DReference;
        }
    }
}