using UnityEngine;

namespace StellarMass.Input
{
    public static class Inputs
    {
        public static KeyCode Accept = KeyCode.Return;
        public static KeyCode ThrustForward = KeyCode.W;
        public static KeyCode TurnLeft = KeyCode.A;
        public static KeyCode TurnRight = KeyCode.D;
        public static KeyCode Shoot = KeyCode.Space;
        public static KeyCode[] AllInputs => new[]
        {
            Accept,
            ThrustForward,
            TurnLeft,
            TurnRight,
            Shoot,
        };
    }
}