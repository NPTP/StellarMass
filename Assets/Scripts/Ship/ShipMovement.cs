using System.Collections;
using StellarMass.Input;
using StellarMass.Utilities.Attributes;
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
        [SerializeField] private SpriteShapeRenderer jetsRenderer;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float flickerTime = 0.01f;
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float turnSpeed;

        private bool thrusting;

        private void Awake()
        {
            playerCollider2DReference = playerCollider2D;
        }

        private void Start()
        {
            jetsRenderer.enabled = false;
            InputReceiver.AddListeners(InputType.ThrustForward, StartThrustForward, StopThrustForward);
            InputReceiver.AddListeners(InputType.Shoot, Shoot);
            InputReceiver.AddListeners(InputType.Hyperspace, Hyperspace);
        }

        private void OnDestroy()
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
            Debug.Log(nameof(Hyperspace));
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

        private void StopThrustForward()
        {
            thrusting = false;
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