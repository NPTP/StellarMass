using StellarMass.Game.Ship.Bullet.States;
using StellarMass.Systems.StateMachines;
using StellarMass.Utilities.Attributes;
using UnityEngine;

namespace StellarMass.Game.Ship.Bullet
{
    [RequireComponent(typeof(StateMachine))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] [Required] private StateMachine stateMachine;
        [SerializeField] [Required] private Collider2D col2D;
        [SerializeField] private GameObject bulletTrailPrefab;
        [SerializeField] private SpriteRenderer[] spriteRenderers;
        
        private float elapsedTimeAlive;
        private float elapsedTimeSinceLastTrail;

        private void OnValidate()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            stateMachine.QueueState(new BulletFlyState(bulletTrailPrefab, transform, spriteRenderers, col2D));
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            stateMachine.QueueState(new BulletCollideState(gameObject, collision2D));
        }
    }
}