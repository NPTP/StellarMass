using StellarMass.Data;
using UnityEngine;

namespace StellarMass.Ship
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject bulletTrailPrefab;
        [SerializeField] private Transform lensFlare;
        
        private float elapsedTimeSinceLastTrail;

        private void Start()
        {
            Destroy(gameObject, RTD.Player.BulletLifetime);
        }

        private void Update()
        {
            Transform thisTransform = transform;

            if (elapsedTimeSinceLastTrail >= RTD.Player.TrailFrequency)
            {
                elapsedTimeSinceLastTrail = 0;
                Instantiate(bulletTrailPrefab, thisTransform.position, thisTransform.rotation);
            }

            thisTransform.position += thisTransform.up * (RTD.Player.BulletSpeed * Time.deltaTime);
            elapsedTimeSinceLastTrail += Time.deltaTime;
            
            lensFlare.rotation = Quaternion.identity;
        }
    }
}