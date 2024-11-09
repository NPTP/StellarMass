using DG.Tweening;
using UnityEngine;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.StateMachines;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletExpireState : State
    {
        private readonly SpriteRenderer[] spriteRenderers;
        private readonly Collider2D collider;

        public BulletExpireState(SpriteRenderer[] spriteRenderers, Collider2D collider)
        {
            this.collider = collider;
            this.spriteRenderers = spriteRenderers;
        }

        public override void Begin()
        {
            base.Begin();
            
            collider.enabled = false;
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];
                Tween t = spriteRenderer.DOFade(0, PersistentData.Player.BulletExpirationFadeTime);
                if (i == 0)
                {
                    t.OnComplete(End);
                }
            }
        }
        
        public override void End()
        {
            base.End();
            Object.Destroy(collider.gameObject);
        }
    }
}