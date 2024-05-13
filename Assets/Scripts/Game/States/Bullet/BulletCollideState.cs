using StellarMass.Systems.StateMachines;
using UnityEngine;

namespace StellarMass.Game.States.Bullet
{
    public class BulletCollideState : State
    {
        private readonly GameObject bulletGameObject;
        
        public BulletCollideState(GameObject bulletGameObject)
        {
            this.bulletGameObject = bulletGameObject;
        }

        public override void Begin()
        {
            base.Begin();
            
            Object.Destroy(bulletGameObject);
        }
    }
}