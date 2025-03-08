using System;
using UnityEngine;

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
            Type = settings.ID.Internal;
        }

        public readonly AchievementsTypes Type;
        public AchievementPointsStep PointsStep => _pointsStep;
        public string Name => _settings.Name;
        public string Description => _settings.Description;
        public Sprite Image => _settings.Image;

        public event Action ChangedStep;
        public event Action<int> ChangedPoints;
        public event Action Reached;

        public void Apply(int points)
        {
            _pointsStep.Points += points;

            if (_settings.CheckReachedNewSteps(_pointsStep.Points, ref _pointsStep.Step))
            {
                ChangedStep?.Invoke();

                if (_settings.CheckReachedAchievement(_pointsStep.Step))
                    Reached?.Invoke();
            }

            ChangedPoints?.Invoke(_pointsStep.Points);
        }
    }
}