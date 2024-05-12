using StellarMass.Systems.Data;
using UnityEngine;

namespace StellarMass.Game.Data
{
    [CreateAssetMenu]
    public class PlayerData : DataScriptable
    {
        [Header("Player Ship")]
        
        [SerializeField] private float thrustFlickerTime = 0.04f;
        public float ThrustFlickerTime => thrustFlickerTime;
        
        [SerializeField] private float forwardForce = 200f;
        public float ForwardForce => forwardForce;
        
        [SerializeField] private float maxVelocityMagnitude = 10f;
        public float MaxVelocityMagnitude => maxVelocityMagnitude;
        
        [SerializeField] private float turnSpeed = 25f;
        public float TurnSpeed => turnSpeed;
        
        [SerializeField] private float hyperspaceTime = 0.5f;
        public float HyperspaceTime => hyperspaceTime;
        
        [SerializeField] private float shootCooldown = 0.1f;
        public float ShootCooldown => shootCooldown;

        [Header("Bullet")]
        
        [SerializeField] private float bulletLifetime = 1f;
        public float BulletLifetime => bulletLifetime;

        [SerializeField] private float bulletSpeed = 15f;
        public float BulletSpeed => bulletSpeed;

        [SerializeField] private float trailFrequency = 0.01f;
        public float TrailFrequency => trailFrequency;
    }
}