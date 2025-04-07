using System;

namespace BallzMerge.Achievement
{
    public abstract class AchievementObserverBase : IDisposable
    {
        protected readonly int Count;
        public readonly AchievementProperty Property;

        public event Action<AchievementsTypes, AchievementPointsStep, AchievementData> ReachedStep;
        public event Action<AchievementsTypes, int> ChangedPoints;
        public event Action<AchievementsTypes, AchievementData> ReachedAchievement;

        public AchievementObserverBase(AchievementSettings settings, AchievementPointsStep pointsStep)
        {
            Property = new AchievementProperty(settings, pointsStep);
            Property.ChangedStep += OnStepChanged;
            Property.ChangedPoints += OnPointsChanged;
            Property.Reached += OnAchievementReached;
            Count = 1;
        }

        public void Dispose()
        {
            Property.ChangedStep -= OnStepChanged;
            Property.ChangedPoints -= OnPointsChanged;
            Property.Reached -= OnAchievementReached;
            Destruct();
        }

        protected abstract void Destruct();

        public abstract void Construct();

        private void OnStepChanged(AchievementsTypes achievementType, AchievementPointsStep pointsStep, AchievementData achievementData)
        {
            ReachedStep?.Invoke(achievementType, pointsStep, achievementData);
        }

        private void OnPointsChanged(AchievementsTypes achievementType, int count)
        {
            ChangedPoints?.Invoke(achievementType, count);
        }

        protected void OnAchievementReached(AchievementsTypes achievementType, AchievementData achievementData)
        {
            ReachedAchievement?.Invoke(achievementType, achievementData);
            Dispose();
        }
    }
}