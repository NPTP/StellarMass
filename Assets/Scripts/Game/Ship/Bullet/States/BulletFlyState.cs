using Summoner.Game.VFX;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.ObjectPooling;
using Summoner.Systems.StateMachines;
using Summoner.Utilities;
using Summoner.Utilities.Extensions;
using UnityEngine;

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

        public override void BeginState()
        {
            spriteRenderers.For(sr => sr.color = sr.color.SetValues(a: 1));
        }

        public override bool UpdateState(out State next)
        {
            if (elapsedTimeSinceLastTrail >= PersistentData.Player.TrailFrequency)
            {
                elapsedTimeSinceLastTrail = 0;
                ObjectPooler.Instantiate(bulletTrailPrefab, bulletTransform.position, bulletTransform.rotation)
                    .FadeOut(0.75f, PD.Player.TrailFadeTime, Curve.Type.EaseOutExp);
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