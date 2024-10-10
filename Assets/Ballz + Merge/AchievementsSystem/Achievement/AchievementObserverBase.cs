using System;

public abstract class AchievementObserverBase : IDisposable
{
    public AchievementObserverBase(AchievementSettings settings)
    {
        Settings = settings;
    }

    public readonly AchievementSettings Settings;

    public event Action<int> Reached;

    public void Dispose()
    {
        Destruct();
    }

    protected void TriggerReached(int target) => Reached?.Invoke(target);

    protected abstract void Destruct();
}
