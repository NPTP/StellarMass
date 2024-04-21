using StellarMass.ScreenLoop;
using StellarMass.Utilities.Attributes;
using UnityEngine;

namespace StellarMass.Ship
{
    public class Bullet : MonoBehaviour
    {
        private const float TIME_BETWEEN_TRAILS = 0.01f;
        private const float LIFETIME = 2f;
        private const float SPEED = 15f;

        [SerializeField][Required] private LoopableCollider loopableCollider;
        [SerializeField] private GameObject bulletTrailPrefab;
        [SerializeField] private Transform lensFlare;
        
        private float elapsedTimeSinceLastTrail;

        private void Start()
        {
            Destroy(this.gameObject, LIFETIME);
            // NP TODO: clean up. Why doesn't it get loopable collider'd?
            loopableCollider.DisableCollider();
            loopableCollider.EnableCollider();
        }

        private void Update()
        {
            Transform thisTransform = transform;

            if (elapsedTimeSinceLastTrail >= TIME_BETWEEN_TRAILS)
            {
                elapsedTimeSinceLastTrail = 0;
                Instantiate(bulletTrailPrefab, thisTransform.position, thisTransform.rotation);
            }

            thisTransform.position += thisTransform.up * SPEED * Time.deltaTime;
            elapsedTimeSinceLastTrail += Time.deltaTime;
            
            lensFlare.rotation = Quaternion.identity;
        }
    }
}