using Summoner.Systems.Data.Persistent;
using UnityEngine;

namespace Summoner.Game.Ship.States
{
    public class ShipShootState : ShipState
    {
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