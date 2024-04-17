using System.Collections;
using StellarMass.Input;
using StellarMass.Utilities.Attributes;
using UnityEngine;
using UnityEngine.U2D;

namespace StellarMass.Ship
{
    public class ShipMovement : MonoBehaviour
    {
        [SerializeField][Required] private Rigidbody2D shipRb;
        [Space]
        [SerializeField] private SpriteShapeRenderer jetsRenderer;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float flickerTime = 0.01f;
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float turnSpeed;

        private bool thrusting;

        private void Start()
        {
            jetsRenderer.enabled = false;
            InputReceiver.AddListeners(Inputs.ThrustForward, StartThrustForward, StopThrustForward);
            InputReceiver.AddListeners(Inputs.Shoot, Shoot);
        }

        private void OnDestroy()
        {
            InputReceiver.RemoveListeners(Inputs.ThrustForward, StartThrustForward, StopThrustForward);
            InputReceiver.RemoveListeners(Inputs.Shoot, Shoot);
        }

        private void Update()
        {
            if (InputReceiver.GetKeyDown(Inputs.TurnLeft))
            {
                Turn(left: true);
            }
        
            if (InputReceiver.GetKeyDown(Inputs.TurnRight))
            {
                Turn(left: false);
            }
        }

        private void Shoot()
        {
            Instantiate(bulletPrefab, transform.position, transform.rotation);
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