using System;
using UnityEngine;

public abstract class BallComponent : MonoBehaviour
{
    public event Action Triggered;

    public virtual void ChangeActivity(bool state)
    {
        enabled = state;
    }

    protected Rigidbody2D GetRigidbody()
    {
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb) == false)
            Debug.LogError($"Achtung!! Ball {name} without Rigidbody!");

        return rb;
    }

    protected void ActivateTrigger() => Triggered?.Invoke();
}