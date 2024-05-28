using StellarMass.Systems.Data;
using UnityEngine;

namespace StellarMass.Game.Data
{
    [CreateAssetMenu]
    public class PlayerData : RuntimeData<PlayerData>
    {
        [Header("Player Ship")]
        
        [SerializeField] private float thrustFlickerTime = 0.04f;
        public static float ThrustFlickerTime => Instance.thrustFlickerTime;
        
        [SerializeField] private float forwardForce = 200f;
        public static float ForwardForce => Instance.forwardForce;
        
        [SerializeField] private float maxVelocityMagnitude = 10f;
        public static float MaxVelocityMagnitude => Instance.maxVelocityMagnitude;
        
        [SerializeField] private float turnSpeed = 25f;
        public static float TurnSpeed => Instance.turnSpeed;
        
        [SerializeField] private float hyperspaceTime = 0.5f;
        public static float HyperspaceTime => Instance.hyperspaceTime;
        
        [SerializeField] private float shootCooldown = 0.1f;
        public static float ShootCooldown => Instance.shootCooldown;

        [Header("Bullet")]
        
        [SerializeField] private float bulletLifetime = 1f;
        public static float BulletLifetime => Instance.bulletLifetime;

        [SerializeField] private float bulletSpeed = 15f;
        public static float BulletSpeed => Instance.bulletSpeed;

        [SerializeField] private float trailFrequency = 0.01f;
        public static float TrailFrequency => Instance.trailFrequency;
        
        [SerializeField] private float trailFadeTime = 0.4f;
        public static float TrailFadeTime => Instance.trailFadeTime;

        [SerializeField] private float bulletExpirationFadeTime = 0.15f;
        public static float BulletExpirationFadeTime => Instance.bulletExpirationFadeTime;
    }
}