using UnityEngine;

namespace StellarMass.ScreenLoop
{
    public class ScreenLoop : MonoBehaviour
    {
        private const float BUFFER_SPACING = 0f;
        private const int X = 0;
        private const int Y = 1;
        
        [SerializeField] private ScreenLoopBoundaryTrigger box;

        private void OnEnable()
        {
            box.OnEntered += HandleEntered;
            box.OnExited += HandleExited;
        }

        private void OnDisable()
        {
            box.OnEntered -= HandleEntered;
            box.OnExited -= HandleExited;
        }

        private void HandleLoop(Collider2D col, int index, float value)
        {
            Transform colTransform = col.transform;
            Vector3 pos = colTransform.position;
            pos[index] = value;
            colTransform.position = pos;
        }
        
        private void HandleEntered(Collider2D col)
        {
        }

        private void HandleExited(Collider2D col)
        {
            Vector2 pos = col.transform.position;
            Vector2 boxPos = box.transform.position;

            float yMax = boxPos.y + box.Bounds.extents.y;
            float yMin = boxPos.y - box.Bounds.extents.y;
            float xMax = boxPos.x + box.Bounds.extents.x;
            float xMin = boxPos.x - box.Bounds.extents.x;
            
            if (pos.y > yMax) HandleLoop(col, Y, transform.position.y - box.Bounds.extents.y + BUFFER_SPACING);
            else if (pos.y < yMin) HandleLoop(col, Y, transform.position.y + box.Bounds.extents.y - BUFFER_SPACING);
            else if (pos.x > xMax) HandleLoop(col, X, transform.position.x - box.Bounds.extents.x + BUFFER_SPACING);
            else if (pos.x < xMin) HandleLoop(col, X, transform.position.x + box.Bounds.extents.x - BUFFER_SPACING);
        }
    }
}