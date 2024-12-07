using UnityEngine;
using Summoner.Systems.StateMachines;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletExpireState : StateInstance
    {
        public readonly SpriteRenderer[] spriteRenderers;
        public readonly Collider2D collider;

        public BulletExpireState(SpriteRenderer[] spriteRenderers, Collider2D collider)
        {
            this.collider = collider;
            this.spriteRenderers = spriteRenderers;
        }
    }
}