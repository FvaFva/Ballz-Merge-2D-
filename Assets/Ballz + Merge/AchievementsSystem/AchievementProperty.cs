using System;

public struct AchievementProperty
{
    public AchievementSettings Settings;
    public int Current;

    public event Action<int> Reached;

    public void Apply(int count)
    {
        Current += count;
        int result = Settings.CheckReachedTarget(Current);

        if (result != 0)
            Reached?.Invoke(result);
    }
}
