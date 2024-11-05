using StellarMass.Systems.StateMachines;
using UnityEngine;
using StellarMass.Systems.Data.Persistent;

namespace StellarMass.Game.Ship.Bullet.States
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
        
        public override State Update()
        {
            base.Update();

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
                return new BulletExpireState(spriteRenderers, collider);
            }

            return null;
        }
    }
}