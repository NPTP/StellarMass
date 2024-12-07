using Summoner.Systems.StateMachines;
using UnityEngine;

namespace Summoner.Game.Ship.Bullet.States
{
    public class BulletCollideState : StateInstance
    {
        public readonly GameObject bulletGameObject;
        
        public BulletCollideState(GameObject bulletGameObject)
        {
            this.bulletGameObject = bulletGameObject;
        }
    }
}