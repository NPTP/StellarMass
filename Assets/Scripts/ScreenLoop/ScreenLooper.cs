using System.Collections.Generic;
using UnityEngine;

namespace StellarMass.ScreenLoop
{
    public class ScreenLooper : MonoBehaviour
    {
        private const float BUFFER_SPACING = 0f;
        private const int X = 0;
        private const int Y = 1;
        
        [SerializeField] private ScreenLoopBoundaryTrigger top;
        [SerializeField] private ScreenLoopBoundaryTrigger bottom;
        [SerializeField] private ScreenLoopBoundaryTrigger left;
        [SerializeField] private ScreenLoopBoundaryTrigger right;
        
        private readonly HashSet<Collider2D> ignoredTop = new();
        private readonly HashSet<Collider2D> ignoredBottom = new();
        private readonly HashSet<Collider2D> ignoredLeft = new();
        private readonly HashSet<Collider2D> ignoredRight = new();

        private void Awake()
        {
            top.OnEntered += HandleEnteredTop;
            top.OnExited += HandleExitedTop;
            bottom.OnEntered += HandleEnteredBottom;
            bottom.OnExited += HandleExitedBottom;
            left.OnEntered += HandleEnteredLeft;
            left.OnExited += HandleExitedLeft;
            right.OnEntered += HandleEnteredRight;
            right.OnExited += HandleExitedRight;
        }

        private void OnDisable()
        {
            ignoredTop.Clear();
            ignoredBottom.Clear();
            ignoredLeft.Clear();
            ignoredRight.Clear();
        }

        private void OnDestroy()
        {
            top.OnEntered -= HandleEnteredTop;
            top.OnExited -= HandleExitedTop;
            bottom.OnEntered -= HandleEnteredBottom;
            bottom.OnExited -= HandleExitedBottom;
            left.OnEntered -= HandleEnteredLeft;
            left.OnExited -= HandleExitedLeft;
            right.OnEntered -= HandleEnteredRight;
            right.OnExited -= HandleExitedRight;
        }

        private void HandleLoop(Collider2D col, int index, float value)
        {
            Transform colTransform = col.transform;
            Vector3 pos = colTransform.position;
            pos[index] = value;
            colTransform.position = pos;
        }
        
        private void HandleEnteredTop(Collider2D col)
        {
            if (ignoredTop.Contains(col)) return;
            ignoredBottom.Add(col);
            HandleLoop(col, Y, bottom.transform.position.y + bottom.Bounds.extents.y + BUFFER_SPACING);
        }

        private void HandleEnteredBottom(Collider2D col)
        {
            if (ignoredBottom.Contains(col)) return;
            ignoredTop.Add(col);
            HandleLoop(col, Y, top.transform.position.y - top.Bounds.extents.y - BUFFER_SPACING);
        }

        private void HandleEnteredLeft(Collider2D col)
        {
            if (ignoredLeft.Contains(col)) return;
            ignoredRight.Add(col);
            HandleLoop(col, X, right.transform.position.x - right.Bounds.extents.x - BUFFER_SPACING);
        }

        private void HandleEnteredRight(Collider2D col)
        {
            if (ignoredRight.Contains(col)) return;
            ignoredLeft.Add(col);
            HandleLoop(col, X, left.transform.position.x + left.Bounds.extents.x + BUFFER_SPACING);
        }
        
        private void HandleExitedTop(Collider2D col)
        {
            ignoredTop.Remove(col);
        }

        private void HandleExitedBottom(Collider2D col)
        {
            ignoredBottom.Remove(col);
        }

        private void HandleExitedLeft(Collider2D col)
        {
            ignoredLeft.Remove(col);
        }

        private void HandleExitedRight(Collider2D col)
        {
            ignoredRight.Remove(col);
        }
    }
}