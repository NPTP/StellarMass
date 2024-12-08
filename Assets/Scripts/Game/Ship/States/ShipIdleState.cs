using Summoner.Systems.Data.Persistent;
using UnityEngine;

namespace Summoner.Game.Ship.States
{
    public class ShipIdleState : ShipState
    {
        public ShipIdleState(ShipControl ship) : base(ship) { }

        public override void UpdateState()
        {
            TurnUpdate();
        }
        
        public override void FixedUpdateState()
        {
            ThrustUpdate();
            ClampRigidBodyVelocity();
        }
        
        private void TurnUpdate()
        {
            if (ship.Turning)
            {
                if (ship.Direction < 0)
                    turn(left: true);
                else if (ship.Direction > 0)
                    turn(left: false);
            }
            
            void turn(bool left)
            {
                ship.ShipRigidBody.transform.rotation *= Quaternion.Euler(new Vector3(0, 0,
                    (left ? 1 : -1) * 10 * (PersistentData.Player.TurnSpeed * Time.deltaTime)));
            }
        }
        
        private void ThrustUpdate()
        {
            if (ship.Thrusting)
            {
                if (ship.FlickerElapsed == 0)
                {
                    ship.JetsRenderer.enabled = true;
                }
                else if (ship.FlickerElapsed >= PersistentData.Player.ThrustFlickerTime)
                {
                    ship.FlickerElapsed = 0;
                    ship.JetsRenderer.enabled = !ship.JetsRenderer.enabled;
                }

                ship.FlickerElapsed += Time.fixedDeltaTime;

                physicsThrust(negative: false);
            }
            else
            {
                ship.FlickerElapsed = 0;
                ship.JetsRenderer.enabled = false;
            }
            
            void physicsThrust(bool negative)
            {
                ship.ShipRigidBody.AddForce(
                    ship.ShipRigidBody.transform.up *
                    ((negative ? -1 : 1) * (PersistentData.Player.ForwardForce * Time.fixedDeltaTime)),
                    ForceMode2D.Force);
            }
        }

        private void ClampRigidBodyVelocity()
        {
            Vector2 shipVelocity = ship.ShipRigidBody.velocity;
            if (shipVelocity.sqrMagnitude > ship.SqrMaxVelocityMagnitude)
            {
                ship.ShipRigidBody.velocity = shipVelocity.normalized * PD.Player.MaxVelocityMagnitude;
            }
        }
    }
}