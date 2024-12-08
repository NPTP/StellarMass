using System.Collections;
using Summoner.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using Input = NPTP.InputSystemWrapper.Input;
using Random = UnityEngine.Random;
using Summoner.Game.ScreenLoop;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.MonoReferences;
using Summoner.Systems.ObjectPooling;
using Summoner.Utilities.Attributes;

namespace Summoner.Game.Ship
{
    public class ShipControl : MonoBehaviour
    {
        private static Collider2D playerCollider2DReference;
        public static Collider2D PlayerCollider2DReference => playerCollider2DReference;
        
        [SerializeField][Required] private Rigidbody2D shipRb;
        [SerializeField][Required] private Collider2D playerCollider2D;
        [Space]
        [SerializeField] private SpriteShapeRenderer jetsRenderer;
        [SerializeField] private GameObject bulletPrefab;

        private float sqrMaxVelocityMagnitude;
        private float lastShotTime;
        private bool thrusting;
        private bool turning;

        private void Awake()
        {
            playerCollider2DReference = playerCollider2D;
            sqrMaxVelocityMagnitude = PersistentData.Player.MaxVelocityMagnitude.Squared();
            ObjectPooler.PrePopulatePool(bulletPrefab, 10);
            // TODO: Bullet trail prefab prepopulate too
        }

        private void Start()
        {
            jetsRenderer.enabled = false;
            AddInputListeners();
        }

        private void OnDisable()
        {
            thrusting = false;
            turning = false;
        }

        private void OnDestroy()
        {
            RemoveInputListeners();
        }

        private void AddInputListeners()
        {
            Input.Gameplay.Thrust.OnEvent += OnThrust;
            Input.Gameplay.Shoot.OnEvent += OnShoot;
            Input.Gameplay.Turn.OnEvent += OnTurn;
            Input.Gameplay.Hyperspace.OnEvent += OnHyperspace;
        }

        private void RemoveInputListeners()
        {
            Input.Gameplay.Thrust.OnEvent -= OnThrust;
            Input.Gameplay.Shoot.OnEvent -= OnShoot;
            Input.Gameplay.Turn.OnEvent -= OnTurn;
            Input.Gameplay.Hyperspace.OnEvent -= OnHyperspace;
        }

        private void FixedUpdate()
        {
            ClampRbVelocity();
        }

        private void ClampRbVelocity()
        {
            Vector2 shipVelocity = shipRb.velocity;
            if (shipVelocity.sqrMagnitude > sqrMaxVelocityMagnitude)
            {
                shipRb.velocity = shipVelocity.normalized * PersistentData.Player.MaxVelocityMagnitude;
            }
        }

        private void OnShoot(InputAction.CallbackContext context)
        {
            if (!context.started)
            {
                return;
            }

            if (Time.time - lastShotTime >= PersistentData.Player.ShootCooldown)
            {
                lastShotTime = Time.time;
                ObjectPooler.Instantiate(bulletPrefab, transform.position, transform.rotation);
            }
        }
        
        private void OnTurn(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    turning = true;
                    StartCoroutine(turnRoutine());
                    break;
                case InputActionPhase.Canceled:
                    turning = false;
                    break;
            }

            IEnumerator turnRoutine()
            {
                float dir = context.ReadValue<float>();
                while (turning)
                {
                    if (dir < 0) Turn(left: true);
                    else if (dir > 0) Turn(left: false);
                    yield return null;
                }
            }
        }

        private void OnHyperspace(InputAction.CallbackContext context)
        {
            if (!context.started)
            {
                return;
            }

            if (!MonoReferenceTable.TryGet(out LoopBoundingBox loopBoundingBox))
            {
                return;
            }
            
            shipRb.isKinematic = true;

            Bounds boxBounds = loopBoundingBox.Bounds;
            Vector3 boxMin = boxBounds.min;
            Vector3 boxMax = boxBounds.max;

            Vector3 shipExtents = playerCollider2D.bounds.extents;
            Transform shipTransform = transform;
            
            shipTransform.position = new Vector3(
                Random.Range(boxMin.x + shipExtents.x, boxMax.x - shipExtents.x),
                Random.Range(boxMin.y + shipExtents.y, boxMax.y - shipExtents.y),
                shipTransform.position.z);
                
            shipRb.isKinematic = false;
        }

        
        private void OnThrust(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    thrusting = true; 
                    StartCoroutine(thrustRoutine());
                    break;
                case InputActionPhase.Canceled:
                    thrusting = false;
                    break;
            }
            
            IEnumerator thrustRoutine()
            {
                jetsRenderer.enabled = true;
                float flickerElapsed = 0f;
                while (thrusting)
                {
                    if (flickerElapsed >= PersistentData.Player.ThrustFlickerTime)
                    {
                        flickerElapsed = 0;
                        jetsRenderer.enabled = !jetsRenderer.enabled;
                    }
                    PhysicsThrust(negative: false);
                    yield return new WaitForFixedUpdate();
                    flickerElapsed += Time.fixedDeltaTime;
                }

                jetsRenderer.enabled = false;
            }
        }
        
        private void PhysicsThrust(bool negative)
        {
            shipRb.AddForce(shipRb.transform.up * ((negative ? -1 : 1) * (PersistentData.Player.ForwardForce * Time.fixedDeltaTime)), ForceMode2D.Force);
        }

        private void Turn(bool left)
        {
            shipRb.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (left ? 1 : -1) * 10 * (PersistentData.Player.TurnSpeed * Time.deltaTime)));
        }
    }
}