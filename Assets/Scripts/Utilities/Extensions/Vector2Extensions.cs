using UnityEngine;

namespace Summoner.Utilities.Extensions
{
    public static class Vector2Extensions
    {
        public static Direction ToDigitalDirection(this Vector2 vector2)
        {
            if (vector2.y == 0)
            {
                if (vector2.x > 0)
                    return Direction.Right;
                if (vector2.x < 0)
                    return Direction.Left;
            }
            else
            {
                if (vector2.y > 0)
                    return Direction.Up;
                if (vector2.y < 0)
                    return Direction.Down;
            }

            return Direction.None;
        }
    }
}