using System;
using UnityEngine;

public abstract class AchievementObserverBase : IDisposable
{
    public AchievementSettings Settings;

    protected int Count;
    protected AchievementProperty Property = new AchievementProperty();

    public event Action Reached;

    public AchievementObserverBase(AchievementSettings settings)
    {
        Settings = settings;
        Settings.Completed += AchievementReached;
        Property.Settings = settings;
        Property.Reached += OnAchievementTargetReached;
        Count = 1;
    }

    public void Dispose()
    {
        Destruct();
    }

    protected virtual void AchievementReached()
    {
        Reached?.Invoke();
        Debug.Log($"Достижение {Settings.Name} получено!");
        Settings.Completed -= AchievementReached;
    }

    protected abstract void OnAchievementTargetReached(int target, int count, int maxTarget);

    protected abstract void Destruct();

    public abstract void Construct();
}
