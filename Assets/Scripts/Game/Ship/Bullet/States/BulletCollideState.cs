using Summoner.Systems.ObjectPooling;
using Summoner.Systems.StateMachines;
using UnityEngine;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletCollideState : State
    {
        private readonly GameObject bulletGameObject;
        
        public BulletCollideState(GameObject bulletGameObject)
        {
            this.bulletGameObject = bulletGameObject;
        }

        public override void BeginState()
        {
            ObjectPooler.Pool(bulletGameObject);
            End();
        }
    }
}