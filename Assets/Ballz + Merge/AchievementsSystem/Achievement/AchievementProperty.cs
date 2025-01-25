using System;

namespace BallzMerge.Achievement
{
    public class AchievementProperty
    {
        private AchievementSettings _settings;
        private AchievementPointsStep _values;

        public AchievementProperty(AchievementSettings settings, AchievementPointsStep pointsStep)
        {
            _settings = settings;
            _values = pointsStep;
            Type = settings.ID.Internal;
        }

        public readonly AchievementsTypes Type;
        public AchievementPointsStep Values => _values;
        public event Action ChangedStep;
        public event Action<int> ChangedPoints;

        public void Apply(int points)
        {
            _values.Points += points;
            int stepsReached = _settings.CheckReachedNewSteps(_values);

            for (int i = 0; i < stepsReached; i++)
            {
                _values.Step++;
                ChangedStep?.Invoke();
            }

            if (stepsReached == 0)
                ChangedPoints?.Invoke(points);
        }
    }
}