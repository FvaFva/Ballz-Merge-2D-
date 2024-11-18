using System;

public class AchievementProperty
{
    private int _current;

    public AchievementSettings Settings;
    public int Target;

    public event Action<int, int, int> Reached;

    public void Apply(int count)
    {
        Target += count;
        int target = Settings.CheckReachedTarget(Target, _current);

        if (target != 0)
        {
            _current++;
            Reached?.Invoke(target, Target, Settings.MaxTargets);
        }

        Settings.CheckDestinationTarget(_current);
    }

    public void Reset() => Target = 0;
}
