using StellarMass.Systems.Data;
using UnityEngine;

namespace StellarMass.Game.Ship
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject bulletTrailPrefab;
        [SerializeField] private SpriteRenderer lensFlareSpriteRenderer;

        private float elapsedTimeAlive;
        private float elapsedTimeSinceLastTrail;
        
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
            
            lensFlareSpriteRenderer.transform.rotation = Quaternion.identity;

            elapsedTimeAlive += Time.deltaTime;
            if (elapsedTimeAlive >= RTD.Player.BulletLifetime)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(this.gameObject);
            // NP TODO: State machine
        }
    }
}