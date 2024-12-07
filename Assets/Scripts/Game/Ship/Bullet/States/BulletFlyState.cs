using Summoner.Systems.Data.Persistent;
using UnityEngine;
using Summoner.Systems.StateMachines;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletFlyState : State
    {
        private readonly GameObject bulletTrailPrefab;
        private readonly Transform bulletTransform;
        private readonly SpriteRenderer[] spriteRenderers;
        private readonly Collider2D collider;
        
        private float elapsedTimeAlive;
        private float elapsedTimeSinceLastTrail;

        public BulletFlyState(GameObject bulletTrailPrefab, Transform bulletTransform, SpriteRenderer[] spriteRenderers, Collider2D collider)
        {
            this.bulletTrailPrefab = bulletTrailPrefab;
            this.bulletTransform = bulletTransform;
            this.spriteRenderers = spriteRenderers;
            this.collider = collider;
        }
        
        public override bool UpdateState(out State next)
        {
            if (elapsedTimeSinceLastTrail >= PersistentData.Player.TrailFrequency)
            {
                elapsedTimeSinceLastTrail = 0;
                Object.Instantiate(bulletTrailPrefab, bulletTransform.position, bulletTransform.rotation);
            }

            bulletTransform.position += bulletTransform.up * (PersistentData.Player.BulletSpeed * Time.deltaTime);
            elapsedTimeSinceLastTrail += Time.deltaTime;

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.transform.rotation = Quaternion.identity;
            }

            elapsedTimeAlive += Time.deltaTime;
            if (elapsedTimeAlive >= PersistentData.Player.BulletLifetime)
            {
                next = new BulletExpireState(spriteRenderers, collider);
                return true;
            }

            next = null;
            return false;
        }
    }
}