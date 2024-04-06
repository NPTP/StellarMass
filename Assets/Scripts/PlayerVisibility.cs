using UnityEngine;

public class PlayerVisibility : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool didIt;
    
    private void Update()
    {
        if (Time.frameCount > 1 && !didIt && !spriteRenderer.isVisible)
        {
            Debug.Log("Did it");
            didIt = true;
            Vector3 dist = spriteRenderer.transform.position;
            spriteRenderer.transform.position = -dist;
        }
    }
}
