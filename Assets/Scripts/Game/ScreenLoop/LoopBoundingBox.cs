using StellarMass.Utilities;
using StellarMass.Utilities.Attributes;
using StellarMass.Utilities.Extensions;
using UnityEngine;

namespace StellarMass.Game.ScreenLoop
{
    public class LoopBoundingBox : ClosedSingleton<LoopBoundingBox>
    {
        private const float BUFFER_SPACING = 0f;
        private const int X_INDEX = 0;
        private const int Y_INDEX = 1;
        
        public static Bounds Bounds => Instance.boxCollider2D.bounds;

        [SerializeField][Required] private BoxCollider2D boxCollider2D;
        [SerializeField][Required] private SpriteRenderer boundsVisualizer;
        public SpriteRenderer BoundsVisualizer => boundsVisualizer;

        protected override void AwakeInitialize()
        {
            base.AwakeInitialize();

            boundsVisualizer.enabled = false;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.IsLoopableCollider())
            {
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
                HandleLoop(col, X_INDEX, transform.position.x - colExtents.x + BUFFER_SPACING);
            else if (colPos.x < xMin)
                HandleLoop(col, X_INDEX, transform.position.x + colExtents.x - BUFFER_SPACING);
            else if (colPos.y > yMax)
                HandleLoop(col, Y_INDEX, transform.position.y - colExtents.y + BUFFER_SPACING);
            else if (colPos.y < yMin)
                HandleLoop(col, Y_INDEX, transform.position.y + colExtents.y - BUFFER_SPACING);
        }
    }
}