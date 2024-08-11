using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallCollisionHandler : BallComponent
{
    private const float MinDelta = 0.0002f;
    private const float ExtraFlip = 0.04f;

    private Rigidbody2D _rb;
    private Transform _transform;

    public event Action<Vector2> Hit;
    public event Action<GridCell, Vector2> HitBlock;
    public event Action GameZoneLeft;

    private void Awake()
    {
        _rb = GetRigidbody();
        _transform = transform;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.contacts[0].point;
        Hit?.Invoke(contactPoint);
        Collider2D hitTarget = collision.collider;

        CorrectingBounceDirection(contactPoint - (Vector2)_transform.position);

        if (hitTarget.TryGetComponent<PlayZoneEdg>(out PlayZoneEdg _))
            GameZoneLeft?.Invoke();
        if (hitTarget.TryGetComponent<GridCell>(out GridCell hitBlock))
            HitBlock?.Invoke(hitBlock, contactPoint);
    }

    private void CorrectingBounceDirection(Vector2 direction)
    {
        direction.Normalize();

        if (Math.Abs(direction.y) < MinDelta)
        {
            Vector2 correctVelocity = _rb.velocity;
            correctVelocity.y = GetSign() * (Math.Abs(correctVelocity.y) + ExtraFlip);
            correctVelocity.x = Math.Sign(correctVelocity.x) * (Math.Abs(correctVelocity.x) - ExtraFlip);
            _rb.velocity = correctVelocity;
        }
        else if (Math.Abs(direction.x) < MinDelta)
        {
            Vector2 correctVelocity = _rb.velocity;
            correctVelocity.x = GetSign() * (Math.Abs(correctVelocity.x) + ExtraFlip);
            correctVelocity.y = Math.Sign(correctVelocity.y) * (Math.Abs(correctVelocity.y) - ExtraFlip);
            _rb.velocity = correctVelocity;
        }
    }

    private int GetSign()
    {
        return UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
    }
}
