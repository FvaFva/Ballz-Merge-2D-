using System;

namespace BallzMerge.Achievement
{
    public class AchievementProperty
    {
        private AchievementSettings _settings;
        private AchievementPointsStep _pointsStep;

        public AchievementProperty(AchievementSettings settings, AchievementPointsStep pointsStep)
        {
            _settings = settings;
            _pointsStep = pointsStep;
        }

        public readonly AchievementsTypes Type;
        public AchievementPointsStep PointsStep => _pointsStep;
        public event Action ChangedStep;
        public event Action<int> ChangedPoints;

        public void Apply(int points)
        {
            _pointsStep.Points += points;
            int stepsReached = _settings.CheckReachedNewSteps(_pointsStep);

            for (int i = 0; i < stepsReached; i++)
            {
                _pointsStep.Step++;
                ChangedStep?.Invoke();
            }

            if (stepsReached == 0)
                ChangedPoints?.Invoke(points);
        }
    }
}