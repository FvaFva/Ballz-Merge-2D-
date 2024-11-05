using System;
using UnityEngine;

public abstract class AchievementObserverBase : IDisposable
{
    public AchievementSettings Settings;

    public event Action Reached;

    public AchievementObserverBase(AchievementSettings settings)
    {
        Settings = settings;
        Settings.Completed += TriggerReached;
    }

    public void Dispose()
    {
        Destruct();
    }

    protected void TriggerReached()
    {
        Reached?.Invoke();
        Debug.Log("Достижение получено!");
        Settings.Completed -= TriggerReached;
    }

    protected abstract void Destruct();
}
