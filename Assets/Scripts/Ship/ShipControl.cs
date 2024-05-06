using System.Collections;
using StellarMass.Data;
using StellarMass.InputManagement;
using StellarMass.LoopBoundaries;
using StellarMass.Utilities;
using StellarMass.Utilities.Attributes;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace StellarMass.Ship
{
    public class ShipControl : TransformCacher
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

        protected override void Awake()
        {
            base.Awake();
            playerCollider2DReference = playerCollider2D;
            sqrMaxVelocityMagnitude = RTD.Player.MaxVelocityMagnitude.Squared();
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
            InputManager.Gameplay.OnThrust += OnThrust;
            InputManager.Gameplay.OnShoot += OnShoot;
            InputManager.Gameplay.OnTurn += OnTurn;
            InputManager.Gameplay.OnHyperspace += OnHyperspace;
        }

        private void RemoveInputListeners()
        {
            InputManager.Gameplay.OnThrust -= OnThrust;
            InputManager.Gameplay.OnShoot -= OnShoot;
            InputManager.Gameplay.OnTurn -= OnTurn;
            InputManager.Gameplay.OnHyperspace -= OnHyperspace;
        }

        private void FixedUpdate()
        {
            ClampRBVelocity();
        }

        private void ClampRBVelocity()
        {
            Vector2 shipVelocity = shipRb.velocity;
            if (shipVelocity.sqrMagnitude > sqrMaxVelocityMagnitude)
            {
                shipRb.velocity = shipVelocity.normalized * RTD.Player.MaxVelocityMagnitude;
            }
        }

        private void OnShoot(InputAction.CallbackContext context)
        {
            if (!context.started)
            {
                return;
            }
            
            if (Time.time - lastShotTime >= RTD.Player.ShootCooldown)
            {
                Instantiate(bulletPrefab, transform.position, transform.rotation);
                lastShotTime = Time.time;
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
            
            shipRb.isKinematic = true;

            Bounds boxBounds = LoopBoundingBox.Bounds;
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
                    if (flickerElapsed >= RTD.Player.ThrustFlickerTime)
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
            shipRb.AddForce(shipRb.transform.up * ((negative ? -1 : 1) * (RTD.Player.ForwardForce * Time.fixedDeltaTime)), ForceMode2D.Force);
        }

        private void Turn(bool left)
        {
            shipRb.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (left ? 1 : -1) * 10 * (RTD.Player.TurnSpeed * Time.deltaTime)));
        }
    }
}