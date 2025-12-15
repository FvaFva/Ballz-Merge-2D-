using System;
using UnityEngine;

public class BallStrikeVectorReader : BallComponent
{
    [SerializeField] private Stick _stick;

    public Vector3 Vector { get; private set; }

    public event Action<Vector3> Changed;
    public event Action<Vector3> Dropped;
    public event Action Canceled;

    private void OnEnable()
    {
        _stick.StickHandled += OnStickHandled;
        _stick.StickValueChanged += OnStickValueChanged;
        _stick.SetActiveIfNotNull(true);
    }

    private void OnDisable()
    {
        _stick.StickHandled -= OnStickHandled;
        _stick.StickValueChanged -= OnStickValueChanged;
        _stick.SetActiveIfNotNull(false);
    }

    private void OnStickHandled(bool isDragging)
    {
        if (isDragging)
        {
            _stick.EnterAim();
            return;
        }

        if (_stick.IsInZone)
        {
            Canceled?.Invoke();
            _stick.EnterMonitoring();
            return;
        }

        OnShot();
    }

    private void OnShot()
    {
        _stick.EnterShooting();
        Vector3 dropVector = Vector;
        Vector = Vector3.zero;
        Dropped?.Invoke(dropVector);
    }

    private void OnStickValueChanged(float newVector)
    {
        float angle = newVector * 180f;
        Vector3 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        if (direction.Equals(Vector) == false)
        {
            Vector = direction;
            Changed?.Invoke(Vector);
        }
    }
}
