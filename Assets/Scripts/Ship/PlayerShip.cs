using UnityEngine;

namespace StellarMass.Ship
{
    public class PlayerShip : MonoBehaviour { }

    public static class PlayerShipExtension
    {
        public static bool IsPlayer(this Collider2D collider2D)
        {
            return collider2D.GetComponent<PlayerShip>() != null;
        }
    }
}