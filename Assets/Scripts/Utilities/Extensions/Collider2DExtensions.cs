using StellarMass.Game.ScreenLoop;
using StellarMass.Game.Ship;
using UnityEngine;

namespace StellarMass.Utilities.Extensions
{
    public static class Collider2DExtensions
    {
        public static bool IsPlayer(this Collider2D collider2D)
        {
            return collider2D == ShipControl.PlayerCollider2DReference;
        }

        public static bool IsLoopableCollider(this Collider2D collider2D)
        {
            return LoopableCollider.IsLoopableCollider(collider2D);
        }
    }
}