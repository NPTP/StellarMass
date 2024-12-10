using System;
using Summoner.Game.ScreenLoop;
using Summoner.Systems.MonoReferences;
using Summoner.Utilities.CurveUtilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Summoner.Game.Ship.States
{
    public class ShipHyperspaceState : ShipState
    {
        protected override Type[] DisallowedTransitions => new[] { typeof(ShipShootState), typeof(ShipHyperspaceState) };

        private const float TIME = 1.0f;
        private Curve disappearCurve = CurveType.EaseInExp;
        private Curve appearCurve = CurveType.EaseOutExp;
        private float timer;
        private float previousValue;

        private Vector2 cachedVelocity;
        private float cachedAngularVelocity;
        
        public ShipHyperspaceState(ShipControl ship) : base(ship) { }

        public override void BeginState()
        {
            ship.ShipRigidBody.isKinematic = true;
            
            cachedVelocity = ship.ShipRigidBody.velocity;
            cachedAngularVelocity = ship.ShipRigidBody.angularVelocity;
            
            ship.ShipRigidBody.velocity = Vector2.zero;
            ship.ShipRigidBody.angularVelocity = 0;
        }

        public override void UpdateState()
        {
            timer += Time.deltaTime;
            
            if (timer < TIME)
            {
                float y = disappearCurve.Evaluate(timer / TIME);
                if (Mathf.Abs(timer - previousValue) >= 0.01f)
                {
                }
            }
            if (timer >= TIME)
            {
                float y = appearCurve.Evaluate(timer / TIME);
                if (Mathf.Abs(timer - previousValue) >= 0.01f)
                {
                }

                if (timer >= TIME)
                {
                    End();
                }
            }

            previousValue = timer;
        }

        protected override void EndState()
        {
            ship.ShipRigidBody.velocity = cachedVelocity;
            ship.ShipRigidBody.angularVelocity = cachedAngularVelocity;
            
            ship.ShipRigidBody.isKinematic = false;
            
            Queue(new ShipFlyState(ship));
        }

        private Vector3 GetNewShipPosition()
        {
            if (!MonoReferenceTable.TryGet(out LoopBoundingBox loopBoundingBox))
            {
                return ship.transform.position;
            }
            
            Bounds boxBounds = loopBoundingBox.Bounds;
            Vector3 boxMin = boxBounds.min;
            Vector3 boxMax = boxBounds.max;

            Vector3 shipExtents = ShipControl.PlayerCollider2DReference.bounds.extents;
            Transform shipTransform = ship.Transform;
            
            return new Vector3(
                Random.Range(boxMin.x + shipExtents.x, boxMax.x - shipExtents.x),
                Random.Range(boxMin.y + shipExtents.y, boxMax.y - shipExtents.y),
                shipTransform.position.z);
        }
    }
}