using System;
using UnityEngine;

namespace BallzMerge.Achievement
{
    public class AchievementProperty
    {
        private AchievementSettings _settings;
        private AchievementPointsStep _pointsStep;
        private readonly AchievementsTypes _type;
        private readonly AchievementData _data;

        public AchievementProperty(AchievementSettings settings, AchievementPointsStep pointsStep)
        {
            _settings = settings;
            _pointsStep = pointsStep;
            _type = settings.ID.Internal;
            _data = new AchievementData(_settings.Name, _settings.Description, _settings.Image, _settings.MaxTargets);
        }

        public AchievementPointsStep PointsStep => _pointsStep;

        public event Action<AchievementsTypes, AchievementPointsStep, AchievementData> ChangedStep;
        public event Action<AchievementsTypes, int> ChangedPoints;
        public event Action<AchievementsTypes, AchievementData> Reached;

        public void Apply(int points)
        {
            _pointsStep.Points += points;

            if (_settings.CheckReachedNewSteps(_pointsStep.Points, ref _pointsStep.Step))
            {
                ChangedStep?.Invoke(_type, _pointsStep, _data);

                if (_settings.CheckReachedAchievement(_pointsStep.Step))
                    Reached?.Invoke(_type, _data);
            }

            ChangedPoints?.Invoke(_type, _pointsStep.Points);
        }
    }
}