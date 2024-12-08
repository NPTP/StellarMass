using Summoner.Game.Ship.Bullet.States;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.StateMachines;
using Summoner.Utilities.Attributes;
using UnityEngine;

namespace Summoner.Game.Ship.Bullet
{
    [RequireComponent(typeof(StateMachine))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] [Required] private StateMachine stateMachine;
        [SerializeField] [Required] private Collider2D col2D;
        [SerializeField] private SpriteRenderer[] spriteRenderers;
        
        private float elapsedTimeAlive;
        private float elapsedTimeSinceLastTrail;

        private void OnValidate()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            stateMachine.Queue(new BulletFlyState(PD.Player.BulletTrailPrefab, transform, spriteRenderers, col2D));
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            stateMachine.Queue(new BulletCollideState(gameObject));
        }
    }
}