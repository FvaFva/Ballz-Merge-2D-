using System;

namespace BallzMerge.Achievement
{
    public abstract class AchievementObserverBase : IDisposable
    {
        protected readonly int Count;
        public readonly AchievementProperty Property;

        public event Action<AchievementsTypes, AchievementPointsStep, AchievementObserverBase> ReachedStep;
        public event Action<AchievementsTypes, int> ChangedPoints;
        public event Action<AchievementsTypes, AchievementObserverBase> ReachedAchievement;

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

        private void OnStepChanged()
        {
            ReachedStep?.Invoke(Property.Type, Property.PointsStep, this);
        }

        private void OnPointsChanged(int count)
        {
            ChangedPoints?.Invoke(Property.Type, count);
        }

        protected void OnAchievementReached()
        {
            ReachedAchievement?.Invoke(Property.Type, this);
            Dispose();
        }
    }
}