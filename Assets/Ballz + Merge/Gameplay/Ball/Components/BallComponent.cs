using System;
using UnityEngine;

public abstract class BallComponent : MonoBehaviour
{
    protected Rigidbody2D MyBody {  get; private set; }

    public event Action Triggered;

    public virtual void ChangeActivity(bool state)
    {
        enabled = state;
    }

    public void SetBody(Rigidbody2D myBody)
    {
        if (MyBody != null) 
            return;

        MyBody = myBody;
    }

    protected void ActivateTrigger() => Triggered?.Invoke();
}