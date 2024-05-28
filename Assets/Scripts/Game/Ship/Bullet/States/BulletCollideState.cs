using StellarMass.Systems.StateMachines;
using UnityEngine;

namespace StellarMass.Game.Ship.Bullet.States
{
    public class BulletCollideState : State
    {
        private readonly GameObject bulletGameObject;
        private readonly Collision2D collision2D;
        
        public BulletCollideState(GameObject bulletGameObject, Collision2D collision2D)
        {
            this.bulletGameObject = bulletGameObject;
            this.collision2D = collision2D;
        }

        public override void Begin()
        {
            base.Begin();
            
            Object.Destroy(bulletGameObject);
        }
    }
}