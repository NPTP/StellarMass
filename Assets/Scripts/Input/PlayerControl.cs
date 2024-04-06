using UnityEngine;

namespace StellarMass.Input
{
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D shipRb;
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float turnSpeed;
        
        private void Update()
        {
            if (UnityEngine.Input.GetKey(KeyCode.W))
            {
                Thrust(negative: false);
            }
            
            if (UnityEngine.Input.GetKey(KeyCode.S))
            {
                Thrust(negative: true);
            }

            if (UnityEngine.Input.GetKey(KeyCode.A))
            {
                Turn(left: true);
            }
        
            if (UnityEngine.Input.GetKey(KeyCode.D))
            {
                Turn(left: false);
            }
        }

        private void Thrust(bool negative)
        {
            shipRb.AddForce(shipRb.transform.up * ((negative ? -1 : 1) * (forwardSpeed * Time.deltaTime)), ForceMode2D.Force);
        }

        private void Turn(bool left)
        {
            shipRb.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (left ? 1 : -1) * 10 * (turnSpeed * Time.deltaTime)));
        }
    }
}