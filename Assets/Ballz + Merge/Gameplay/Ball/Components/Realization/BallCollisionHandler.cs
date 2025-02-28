using System;
using UnityEngine;
using BallzMerge.Gameplay.Level;
using BallzMerge.Gameplay.BlockSpace;

namespace BallzMerge.Gameplay.BallSpace
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallCollisionHandler : BallComponent
    {
        private const float MinDelta = 0.0002f;
        private const float ExtraFlip = 0.09f;

        private Transform _transform;

        public event Action<Vector2> Hit;
        public event Action NonBlockHit;
        public event Action GameZoneLeft;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 contactPoint = collision.contacts[0].point;
            Hit?.Invoke(contactPoint);
            Collider2D hitTarget = collision.collider;

            CorrectingBounceDirection(contactPoint - (Vector2)_transform.position);

            if (hitTarget.TryGetComponent(out BlockPhysicModel hitBlock))
                hitBlock.Kick(contactPoint.CalculateDirection(hitBlock.WorldPosition));
            else
                NonBlockHit?.Invoke();

            if (hitTarget.TryGetComponent(out PlayZoneEdge _))
                GameZoneLeft?.Invoke();
        }

        private void CorrectingBounceDirection(Vector2 direction)
        {
            direction.Normalize();

            if (Math.Abs(direction.y) < MinDelta)
            {
                Vector2 correctVelocity = MyBody.velocity;
                correctVelocity.y = (Math.Abs(correctVelocity.y) + ExtraFlip) * -1;
                correctVelocity.x = Math.Sign(correctVelocity.x) * (Math.Abs(correctVelocity.x) - ExtraFlip);
                MyBody.velocity = correctVelocity;
            }
            else if (Math.Abs(direction.x) < MinDelta)
            {
                Vector2 correctVelocity = MyBody.velocity;
                correctVelocity.x = (Math.Abs(correctVelocity.x) + ExtraFlip) * -1;
                correctVelocity.y = Math.Sign(correctVelocity.y) * (Math.Abs(correctVelocity.y) - ExtraFlip);
                MyBody.velocity = correctVelocity;
            }
        }
    }
}