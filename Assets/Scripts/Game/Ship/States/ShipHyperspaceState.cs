using Summoner.Game.ScreenLoop;
using Summoner.Systems.MonoReferences;
using UnityEngine;

namespace Summoner.Game.Ship.States
{
    public class ShipHyperspaceState : ShipState
    {
        public ShipHyperspaceState(ShipControl ship) : base(ship) { }

        public override void BeginState()
        {
            if (!MonoReferenceTable.TryGet(out LoopBoundingBox loopBoundingBox))
            {
                return;
            }
            
            ship.ShipRigidBody.isKinematic = true;

            Bounds boxBounds = loopBoundingBox.Bounds;
            Vector3 boxMin = boxBounds.min;
            Vector3 boxMax = boxBounds.max;

            Vector3 shipExtents = ship.PlayerCollider2D.bounds.extents;
            Transform shipTransform = ship.Transform;
            
            shipTransform.position = new Vector3(
                Random.Range(boxMin.x + shipExtents.x, boxMax.x - shipExtents.x),
                Random.Range(boxMin.y + shipExtents.y, boxMax.y - shipExtents.y),
                shipTransform.position.z);
                
            ship.ShipRigidBody.isKinematic = false;
            
            Queue(new ShipFlyState(ship));
        }
    }
}