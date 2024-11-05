using System;

public class AchievementProperty
{
    public AchievementSettings Settings;
    public int Current;

    public event Action<int, int, int> Reached;

    public void Apply(int count)
    {
        Current += count;
        int target = Settings.CheckReachedTarget(Current);

        if (target != 0)
            Reached?.Invoke(target, Current, Settings.MaxTargets);
    }
}
