using Summoner.Game.GameControl;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.MonoReferences;
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
                GameObject bullet = Object.Instantiate(PD.Player.BulletPrefab, ship.Transform.position, ship.Transform.rotation);
                if (MonoReferenceTable.TryGet(out GameController gameController))
                {
                    bullet.transform.SetParent(gameController.SpawnedObjectsParent);
                }
            }
         
            Queue(new ShipIdleState(ship));
        }
    }
}