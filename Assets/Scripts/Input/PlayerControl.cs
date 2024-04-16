using System;
using System.Collections;
using StellarMass.Utilities.Attributes;
using UnityEngine;
using UnityEngine.U2D;

namespace StellarMass.Input
{
    [ExecuteAlways]
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField][Required] private Rigidbody2D shipRb;
        [SerializeField] private SpriteShapeController[] spriteShapeControllers;
        [SerializeField][GUIDisabled] private float spriteLineHeight = 0.5f;
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

        private void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
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
        
        private void Update()
        {
            SetSpriteHeight();

            if (!Application.isPlaying)
            {
                return;
            }
            
            if (InputReceiver.GetKeyDown(Inputs.TurnLeft))
            {
                Turn(left: true);
            }
        
            if (InputReceiver.GetKeyDown(Inputs.TurnRight))
            {
                Turn(left: false);
            }
        }
        
        private void SetSpriteHeight()
        {
            float scale = transform.localScale.x;
            spriteLineHeight = Mathf.Clamp(-(0.2f * scale) + 0.2f, 0.01f, 99f);
            
            for (int i = 0; i < spriteShapeControllers.Length; i++)
            {
                SpriteShapeController spriteShapeController = spriteShapeControllers[i];
                int count = spriteShapeController.spline.GetPointCount();
                for (int j = 0; j < count; j++)
                {
                    spriteShapeController.spline.SetHeight(i, spriteLineHeight);
                }
                spriteShapeController.UpdateSpriteShapeParameters();
            }
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