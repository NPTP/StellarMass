using System.Collections;
using StellarMass.Data;
using StellarMass.OldInput;
using StellarMass.LoopBoundaries;
using StellarMass.Utilities;
using StellarMass.Utilities.Attributes;
using StellarMass.Utilities.Extensions;
using UnityEngine;
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
        private bool thrusting;
        private float lastShotTime;

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

        private void OnDestroy()
        {
            RemoveInputListeners();
        }

        private void AddInputListeners()
        {
            InputReceiver.AddListeners(InputType.ThrustForward, StartThrustForward, StopThrustForward);
            InputReceiver.AddListeners(InputType.Shoot, TryShoot);
            InputReceiver.AddListeners(InputType.Hyperspace, Hyperspace);
        }

        private void RemoveInputListeners()
        {
            InputReceiver.RemoveListeners(InputType.ThrustForward, StartThrustForward, StopThrustForward);
            InputReceiver.RemoveListeners(InputType.Shoot, TryShoot);
            InputReceiver.RemoveListeners(InputType.Hyperspace, Hyperspace);
        }

        private void Update()
        {
            if (InputReceiver.GetKeyDown(InputType.TurnLeft))
            {
                Turn(left: true);
            }
        
            if (InputReceiver.GetKeyDown(InputType.TurnRight))
            {
                Turn(left: false);
            }
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

        private void TryShoot()
        {
            if (Time.time - lastShotTime >= RTD.Player.ShootCooldown)
            {
                Instantiate(bulletPrefab, transform.position, transform.rotation);
                lastShotTime = Time.time;
            }
        }
        
        private void Hyperspace()
        {
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

        private void OnDisable()
        {
            StopThrustForward();
        }

        private void StartThrustForward()
        {
            thrusting = true; 
            StartCoroutine(ThrustRoutine());
        }

        private void StopThrustForward()
        {
            thrusting = false;
        }

        private IEnumerator ThrustRoutine()
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

        private void PhysicsThrust(bool negative)
        {
            shipRb.AddForce(shipRb.transform.up * ((negative ? -1 : 1) * (RTD.Player.ForwardForce * Time.deltaTime)), ForceMode2D.Force);
        }

        private void Turn(bool left)
        {
            shipRb.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (left ? 1 : -1) * 10 * (RTD.Player.TurnSpeed * Time.deltaTime)));
        }
    }
}