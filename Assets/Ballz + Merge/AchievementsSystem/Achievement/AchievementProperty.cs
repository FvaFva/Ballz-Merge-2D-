using System;

namespace BallzMerge.Achievement
{
    public class AchievementProperty
    {
        private AchievementSettings _settings;
        private int _step = 0;
        private int _target = 0;

        public AchievementProperty(AchievementSettings settings)
        {
            _settings = settings;
        }

        public event Action<int, int, int> Reached;

        public void Apply(int count)
        {
            _target += count;
            int target = _settings.CheckReachedTarget(_target, _step);

            if (target != 0)
            {
                _step++;
                Reached?.Invoke(target, _target, _settings.MaxTargets);
            }

            _settings.CheckDestinationTarget(_step);
        }

        public void Reset() => _target = 0;
    }
}