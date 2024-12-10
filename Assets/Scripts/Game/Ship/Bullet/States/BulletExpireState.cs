using Summoner.Systems.Data.Persistent;
using Summoner.Systems.ObjectPooling;
using UnityEngine;
using Summoner.Systems.StateMachines;
using Summoner.Utilities.VFX;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletExpireState : State
    {
        private readonly SpriteRendererGroup spriteRendererGroup;
        private readonly Collider2D collider;

        public BulletExpireState(SpriteRendererGroup spriteRendererGroup, Collider2D collider)
        {
            this.collider = collider;
            this.spriteRendererGroup = spriteRendererGroup;
        }
        
        public override void BeginState()
        {
            collider.enabled = false;
            spriteRendererGroup.Fade(0, PD.Player.BulletExpirationFadeTime, End);
        }
        
        protected override void EndState()
        {
            ObjectPooler.Pool(collider.gameObject);
        }
    }
}