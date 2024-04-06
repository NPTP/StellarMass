using UnityEngine;

namespace StellarMass.Utilities
{
    public class SpriteRendererFillScreen : MonoBehaviour
    {
        [SerializeField] private bool keepAspectRatio;

        public void UpdateFill()
        {
            Camera cam = Camera.main;;
            if (cam == null)
            {
                return;
            }
            
            Vector3 topRightCorner = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));
            float worldSpaceWidth = topRightCorner.x * 2;
            float worldSpaceHeight = topRightCorner.y * 2;

            Vector3 spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size;

            float scaleFactorX = worldSpaceWidth / spriteSize.x;
            float scaleFactorY = worldSpaceHeight / spriteSize.y;

            if (keepAspectRatio)
            {
                if (scaleFactorX > scaleFactorY)
                {
                    scaleFactorY = scaleFactorX;
                }
                else
                {
                    scaleFactorX = scaleFactorY;
                }
            }

            gameObject.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);
        }
    }
}