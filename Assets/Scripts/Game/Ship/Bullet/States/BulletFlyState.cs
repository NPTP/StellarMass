using Summoner.Systems.Data.Persistent;
using Summoner.Systems.ObjectPooling;
using Summoner.Systems.StateMachines;
using Summoner.Utilities;
using Summoner.Utilities.CurveUtilities;
using Summoner.Utilities.Extensions;
using Summoner.Utilities.VFX;
using UnityEngine;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletFlyState : State
    {
        private readonly GameObject bulletTrailPrefab;
        private readonly Transform bulletTransform;
        private readonly SpriteRendererGroup spriteRendererGroup;
        private readonly Collider2D collider;
        
        private float elapsedTimeAlive;
        private float elapsedTimeSinceLastTrail;

        public BulletFlyState(GameObject bulletTrailPrefab, Transform bulletTransform, SpriteRendererGroup spriteRendererGroup, Collider2D collider)
        {
            this.bulletTrailPrefab = bulletTrailPrefab;
            this.bulletTransform = bulletTransform;
            this.spriteRendererGroup = spriteRendererGroup;
            this.collider = collider;
        }
        public override void BeginState()
        {
            spriteRendererGroup.Alpha = 1;
            bulletTransform.ApplyToChildren(child => child.rotation = Quaternion.identity);
        }
        
        public override void UpdateState()
        {
            if (elapsedTimeSinceLastTrail >= PersistentData.Player.TrailFrequency)
            {
                elapsedTimeSinceLastTrail = 0;
                GameObject bulletTrail = ObjectPooler.Instantiate(bulletTrailPrefab, bulletTransform.position, bulletTransform.rotation);
                SpriteRendererGroup group = bulletTrail.GetComponent<SpriteRendererGroup>();
                group.Alpha = 1;
                group.Fade(0, PD.Player.TrailFadeTime, () => ObjectPooler.Pool(bulletTrail))
                    .SetCurve(CurveType.EaseOutExp);
            }

            bulletTransform.position += bulletTransform.up * (PersistentData.Player.BulletSpeed * Time.deltaTime);
            elapsedTimeSinceLastTrail += Time.deltaTime;
            
            elapsedTimeAlive += Time.deltaTime;
            if (elapsedTimeAlive >= PersistentData.Player.BulletLifetime)
            {
                Queue(new BulletExpireState(spriteRendererGroup, collider));
            }
        }
    }
}