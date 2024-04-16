using UnityEngine;

namespace StellarMass.Combat
{
    public class Bullet : MonoBehaviour
    {
        private const float TimeBetweenTrails = 0.01f;
        private const float Lifetime = 2f;
        private const float Speed = 15f;

        [SerializeField] private GameObject bulletTrailPrefab;
        [SerializeField] private Transform lensFlare;
        
        private float elapsedTimeSinceLastTrail;

        private void Start()
        {
            Destroy(this.gameObject, Lifetime);
        }

        private void Update()
        {
            Transform thisTransform = transform;

            if (elapsedTimeSinceLastTrail >= TimeBetweenTrails)
            {
                elapsedTimeSinceLastTrail = 0;
                Instantiate(bulletTrailPrefab, thisTransform.position, thisTransform.rotation);
            }

            thisTransform.position += thisTransform.up * Speed * Time.deltaTime;
            elapsedTimeSinceLastTrail += Time.deltaTime;
            
            lensFlare.rotation = Quaternion.identity;
        }
    }
}