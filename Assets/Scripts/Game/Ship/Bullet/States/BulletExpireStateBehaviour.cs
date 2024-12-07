using DG.Tweening;
using UnityEngine;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.StateMachines;

namespace Summoner.Game.Ship.Bullet.States
{
    [CreateAssetMenu(menuName = CREATE_ASSET_PATH + nameof(BulletExpireStateBehaviour), fileName = nameof(BulletExpireStateBehaviour))]
    public class BulletExpireStateBehaviour : StateBehaviour<BulletExpireState>
    {
        public override void Begin(BulletExpireState input)
        {
            input.collider.enabled = false;
            for (int i = 0; i < input.spriteRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = input.spriteRenderers[i];
                Tween t = spriteRenderer.DOFade(0, PersistentData.Player.BulletExpirationFadeTime);
                if (i == 0)
                {
                    t.OnComplete(() => End(input));
                }
            }
        }
        
        public override void End(BulletExpireState input)
        {
            Destroy(input.collider.gameObject);
        }
    }
}