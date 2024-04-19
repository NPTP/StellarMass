using System.Collections;
using StellarMass.Input;
using StellarMass.Utilities.Attributes;
using StellarMass.VFX;
using UnityEngine;
using UnityEngine.U2D;

namespace StellarMass.Ship
{
    public class ShipMovement : MonoBehaviour
    {
        private static Collider2D playerCollider2DReference;
        public static Collider2D PlayerCollider2DReference => playerCollider2DReference;
        
        [SerializeField][Required] private Rigidbody2D shipRb;
        [SerializeField][Required] private Collider2D playerCollider2D;
        [Space]
        [SerializeField] private RendererController rendererController;
        [SerializeField] private SpriteShapeRenderer jetsRenderer;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float flickerTime = 0.01f;
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float turnSpeed;
        [SerializeField] private float hyperspaceTime = 0.5f;

        private bool thrusting;

        private void Awake()
        {
            playerCollider2DReference = playerCollider2D;
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
            InputReceiver.AddListeners(InputType.Shoot, Shoot);
            InputReceiver.AddListeners(InputType.Hyperspace, Hyperspace);
        }

        private void RemoveInputListeners()
        {
            InputReceiver.RemoveListeners(InputType.ThrustForward, StartThrustForward, StopThrustForward);
            InputReceiver.RemoveListeners(InputType.Shoot, Shoot);
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

        private void Shoot()
        {
            Instantiate(bulletPrefab, transform.position, transform.rotation);
        }
        
        private void Hyperspace()
        {
            StartCoroutine(HyperspaceRoutine());
        }

        private IEnumerator HyperspaceRoutine()
        {
            bool wasThrusting = thrusting;
            
            StopThrustForward();
            RemoveInputListeners();
            
            playerCollider2D.enabled = false;
            shipRb.isKinematic = true;
            Vector2 velocity = shipRb.velocity;
            float angularVelocity = shipRb.angularVelocity;
            shipRb.velocity = Vector2.zero;
            shipRb.angularVelocity = 0;
            rendererController.DisableRenderers();

            yield return new WaitForSeconds(hyperspaceTime);
            
            // NP TODO: make this respond to the world boundaries dynamically
            transform.position = new Vector3(Random.Range(-6, 6), Random.Range(-4, 4), transform.position.z);
            
            playerCollider2D.enabled = true;
            shipRb.isKinematic = false;
            shipRb.velocity = velocity;
            shipRb.angularVelocity = angularVelocity;
            rendererController.EnableRenderers();
            
            AddInputListeners();
            
            if (wasThrusting && InputReceiver.GetKeyDown(InputType.ThrustForward))
            {
                StartThrustForward();
            }
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
                if (flickerElapsed >= flickerTime)
                {
                    flickerElapsed = 0;
                    jetsRenderer.enabled = !jetsRenderer.enabled;
                }
                PhysicsThrust(negative: false);
                yield return null;
                flickerElapsed += Time.deltaTime;
            }

            jetsRenderer.enabled = false;
        }

        private void PhysicsThrust(bool negative)
        {
            shipRb.AddForce(shipRb.transform.up * ((negative ? -1 : 1) * (forwardSpeed * Time.deltaTime)), ForceMode2D.Force);
        }

        private void Turn(bool left)
        {
            shipRb.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (left ? 1 : -1) * 10 * (turnSpeed * Time.deltaTime)));
        }
    }
}