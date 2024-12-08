using Summoner.Systems.Data.Persistent;
using Summoner.Systems.ObjectPooling;
using UnityEngine;
using Summoner.Systems.StateMachines;
using Summoner.Utilities.VFX;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletExpireState : State
    {
        private readonly SpriteRendererFadeGroup spriteRendererFadeGroup;
        private readonly Collider2D collider;

        public BulletExpireState(SpriteRendererFadeGroup spriteRendererFadeGroup, Collider2D collider)
        {
            this.collider = collider;
            this.spriteRendererFadeGroup = spriteRendererFadeGroup;
        }
        
        public override void BeginState()
        {
            collider.enabled = false;
            spriteRendererFadeGroup.Fade(0, PD.Player.BulletExpirationFadeTime, End);
        }
        
        protected override void EndState()
        {
            ObjectPooler.Pool(collider.gameObject);
        }
    }
}