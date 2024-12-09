using DG.Tweening;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.ObjectPooling;
using Summoner.Systems.StateMachines;
using Summoner.Utilities.Extensions;
using Summoner.Utilities.VFX;
using UnityEngine;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletFlyState : State
    {
        private readonly GameObject bulletTrailPrefab;
        private readonly Transform bulletTransform;
        private readonly SpriteRendererFadeGroup spriteRendererFadeGroup;
        private readonly Collider2D collider;
        
        private float elapsedTimeAlive;
        private float elapsedTimeSinceLastTrail;

        public BulletFlyState(GameObject bulletTrailPrefab, Transform bulletTransform, SpriteRendererFadeGroup spriteRendererFadeGroup, Collider2D collider)
        {
            this.bulletTrailPrefab = bulletTrailPrefab;
            this.bulletTransform = bulletTransform;
            this.spriteRendererFadeGroup = spriteRendererFadeGroup;
            this.collider = collider;
        }
        public override void BeginState()
        {
            spriteRendererFadeGroup.ResetToInitialAlpha();
            bulletTransform.ApplyToChildren(child => child.rotation = Quaternion.identity);
        }
        
        public override void UpdateState()
        {
            if (elapsedTimeSinceLastTrail >= PersistentData.Player.TrailFrequency)
            {
                elapsedTimeSinceLastTrail = 0;
                GameObject bulletTrail = ObjectPooler.Instantiate(bulletTrailPrefab, bulletTransform.position, bulletTransform.rotation);
                SpriteRendererFadeGroup fadeGroup = bulletTrail.GetComponent<SpriteRendererFadeGroup>();
                fadeGroup.ResetToInitialAlpha();
                fadeGroup.Fade(0, PD.Player.TrailFadeTime, Ease.OutExpo, () => ObjectPooler.Pool(bulletTrail));
            }

            bulletTransform.position += bulletTransform.up * (PersistentData.Player.BulletSpeed * Time.deltaTime);
            elapsedTimeSinceLastTrail += Time.deltaTime;
            
            elapsedTimeAlive += Time.deltaTime;
            if (elapsedTimeAlive >= PersistentData.Player.BulletLifetime)
            {
                Queue(new BulletExpireState(spriteRendererFadeGroup, collider));
            }
        }
    }
}