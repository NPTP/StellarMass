using Summoner.Systems.StateMachines;
using UnityEngine;

namespace Summoner.Game.Ship.Bullet.States
{
    [CreateAssetMenu(menuName = CREATE_ASSET_PATH + nameof(BulletCollideStateBehaviour), fileName = nameof(BulletCollideStateBehaviour))]
    public class BulletCollideStateBehaviour : StateBehaviour<BulletCollideState>
    {
        public override void Begin(BulletCollideState input)
        {
            Destroy(input.bulletGameObject);
        }
    }
}