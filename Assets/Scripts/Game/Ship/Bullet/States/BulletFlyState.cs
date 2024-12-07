using UnityEngine;
using Summoner.Systems.StateMachines;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletFlyState : StateInstance
    {
        public readonly GameObject bulletTrailPrefab;
        public readonly Transform bulletTransform;
        public readonly SpriteRenderer[] spriteRenderers;
        public readonly Collider2D collider;
        
        public float elapsedTimeAlive;
        public float elapsedTimeSinceLastTrail;

        public BulletFlyState(GameObject bulletTrailPrefab, Transform bulletTransform, SpriteRenderer[] spriteRenderers, Collider2D collider)
        {
            this.bulletTrailPrefab = bulletTrailPrefab;
            this.bulletTransform = bulletTransform;
            this.spriteRenderers = spriteRenderers;
            this.collider = collider;
        }
    }
}