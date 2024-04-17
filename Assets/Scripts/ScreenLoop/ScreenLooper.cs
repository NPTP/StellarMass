using UnityEngine;

namespace StellarMass.ScreenLoop
{
    public class ScreenLooper : MonoBehaviour
    {
        private const float BUFFER_SPACING = 0.01f;
        private const int X = 0;
        private const int Y = 1;
        
        [SerializeField] private LoopCollider top;
        [SerializeField] private LoopCollider bottom;
        [SerializeField] private LoopCollider left;
        [SerializeField] private LoopCollider right;

        private void Awake()
        {
            top.OnEntered += HandleEnteredTop;
            bottom.OnEntered += HandleEnteredBottom;
            left.OnEntered += HandleEnteredLeft;
            right.OnEntered += HandleEnteredRight;
        }

        private void OnDestroy()
        {
            top.OnEntered -= HandleEnteredTop;
            bottom.OnEntered -= HandleEnteredBottom;
            left.OnEntered -= HandleEnteredLeft;
            right.OnEntered -= HandleEnteredRight;
        }

        private void HandleLoop(Collider2D player, int index, float value)
        {
            Transform playerTransform = player.transform;
            Vector3 pos = playerTransform.position;
            pos[index] = value;
            playerTransform.position = pos;
        }

        private void HandleEnteredTop(Collider2D player)
        {
            HandleLoop(player, Y, bottom.transform.position.y + bottom.Bounds.extents.y + player.bounds.size.y + BUFFER_SPACING);
        }

        private void HandleEnteredBottom(Collider2D player)
        {
            HandleLoop(player, Y, top.transform.position.y - top.Bounds.extents.y - player.bounds.size.y - BUFFER_SPACING);
        }

        private void HandleEnteredLeft(Collider2D player)
        {
            HandleLoop(player, X, right.transform.position.x - right.Bounds.extents.x - player.bounds.size.x - BUFFER_SPACING);
        }

        private void HandleEnteredRight(Collider2D player)
        {
            HandleLoop(player, X, left.transform.position.x + left.Bounds.extents.x + player.bounds.size.x + BUFFER_SPACING);
        }
    }
}