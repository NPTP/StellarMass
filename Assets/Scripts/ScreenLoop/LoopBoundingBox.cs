using StellarMass.Utilities.Attributes;
using StellarMass.Utilities.Extensions;
using UnityEngine;
using Utilities;

namespace StellarMass.LoopBoundaries
{
    public class LoopBoundingBox : ClosedSingleton<LoopBoundingBox>
    {
        private const float BUFFER_SPACING = 0f;
        private const int X = 0;
        private const int Y = 1;
        
        public static Bounds Bounds => PrivateInstance.boxCollider2D.bounds;

        [SerializeField][Required] private BoxCollider2D boxCollider2D;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.IsLoopableCollider())
            {
                Debug.Log($"Enter: {col.name}");
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.IsLoopableCollider())
            {
                Debug.Log($"Exit: {col.name}");

                OnExited(col);
            }
        }

        private void HandleLoop(Collider2D col, int index, float value)
        {
            Transform colTransform = col.transform;
            Vector3 pos = colTransform.position;
            pos[index] = value;
            colTransform.position = pos;
        }

        private void OnExited(Collider2D col)
        {
            Vector3 boxPos = boxCollider2D.transform.position;
            Vector3 colPos = col.transform.position;
            Vector3 colExtents = boxCollider2D.bounds.extents;

            float yMax = boxPos.y + colExtents.y;
            float yMin = boxPos.y - colExtents.y;
            float xMax = boxPos.x + colExtents.x;
            float xMin = boxPos.x - colExtents.x;
            
            if (colPos.x > xMax)
                HandleLoop(col, X, transform.position.x - colExtents.x + BUFFER_SPACING);
            else if (colPos.x < xMin)
                HandleLoop(col, X, transform.position.x + colExtents.x - BUFFER_SPACING);
            else if (colPos.y > yMax)
                HandleLoop(col, Y, transform.position.y - colExtents.y + BUFFER_SPACING);
            else if (colPos.y < yMin)
                HandleLoop(col, Y, transform.position.y + colExtents.y - BUFFER_SPACING);
        }
    }
}