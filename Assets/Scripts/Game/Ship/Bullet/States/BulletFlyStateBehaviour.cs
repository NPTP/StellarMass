using UnityEngine;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.StateMachines;

namespace Summoner.Game.Ship.Bullet.States
{
    [CreateAssetMenu(menuName = CREATE_ASSET_PATH + nameof(BulletFlyStateBehaviour), fileName = nameof(BulletFlyStateBehaviour))]
    public class BulletFlyStateBehaviour : StateBehaviour<BulletFlyState>
    {
        public override bool UpdateState(BulletFlyState input, out StateInstance next)
        {
            if (input.elapsedTimeSinceLastTrail >= PersistentData.Player.TrailFrequency)
            {
                input.elapsedTimeSinceLastTrail = 0;
                Instantiate(input.bulletTrailPrefab, input.bulletTransform.position, input.bulletTransform.rotation);
            }

            input.bulletTransform.position += input.bulletTransform.up * (PersistentData.Player.BulletSpeed * Time.deltaTime);
            input.elapsedTimeSinceLastTrail += Time.deltaTime;

            foreach (SpriteRenderer spriteRenderer in input.spriteRenderers)
            {
                spriteRenderer.transform.rotation = Quaternion.identity;
            }

            input.elapsedTimeAlive += Time.deltaTime;
            if (input.elapsedTimeAlive >= PersistentData.Player.BulletLifetime)
            {
                next = new BulletExpireState(input.spriteRenderers, input.collider);
                return true;
            }

            next = null;
            return false;
        }
    }
}