using System;
using Summoner.Systems.Data.Persistent;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Summoner.Game.Ship.States
{
    public class ShipShootState : ShipState
    {
        protected override Type[] DisallowedTransitions => new[] { typeof(ShipShootState), typeof(ShipHyperspaceState) };

        public ShipShootState(ShipControl ship) : base(ship) { }
        
        public override void BeginState()
        {
            if (Time.time - ship.LastShotTime >= PersistentData.Player.ShootCooldown)
            {
                ship.LastShotTime = Time.time;
                Object.Instantiate(PD.Player.BulletPrefab, ship.Transform.position, ship.Transform.rotation);
            }
         
            Queue(new ShipFlyState(ship));
        }
    }
}