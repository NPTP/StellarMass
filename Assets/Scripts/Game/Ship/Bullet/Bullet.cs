using Summoner.Game.Ship.Bullet.States;
using Summoner.Systems.StateMachines;
using Summoner.Utilities.Attributes;
using Summoner.Utilities.VFX;
using UnityEngine;

namespace Summoner.Game.Ship.Bullet
{
    [RequireComponent(typeof(StateMachine))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] [Required] private StateMachine stateMachine;
        [SerializeField] [Required] private Collider2D col2D;
        [SerializeField] private GameObject bulletTrailPrefab;
        [SerializeField] private SpriteRendererGroup spriteRendererGroup;

        private float elapsedTimeAlive;
        private float elapsedTimeSinceLastTrail;

        private void OnEnable()
        {
            stateMachine.Queue(new BulletFlyState(bulletTrailPrefab, transform, spriteRendererGroup, col2D));
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            stateMachine.Queue(new BulletCollideState(gameObject));
        }
    }
}