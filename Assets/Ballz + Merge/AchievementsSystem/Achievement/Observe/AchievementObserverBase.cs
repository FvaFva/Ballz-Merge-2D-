using System;

namespace BallzMerge.Achievement
{
    public abstract class AchievementObserverBase : IDisposable
    {
        protected readonly int Count;
        protected readonly AchievementProperty Property;

        public event Action<AchievementsTypes, AchievementPointsStep> ReachedStep;
        public event Action<AchievementsTypes, int> ChangedPoints;

        public AchievementObserverBase(AchievementSettings settings, AchievementPointsStep pointsStep)
        {
            Property = new AchievementProperty(settings, pointsStep);
            Property.ChangedStep += OnStepChanged;
            Property.ChangedPoints += OnPointsChanged;
            Count = 1;
        }

        public void Dispose()
        {
            Property.ChangedStep -= OnStepChanged;
            Property.ChangedPoints -= OnPointsChanged;
            Destruct();
        }

        protected abstract void Destruct();

        public abstract void Construct();

        private void OnStepChanged()
        {
            ReachedStep?.Invoke(Property.Type, Property.PointsStep);
        }

        private void OnPointsChanged(int count)
        {
            ChangedPoints?.Invoke(Property.Type, count);
        }
    }
}