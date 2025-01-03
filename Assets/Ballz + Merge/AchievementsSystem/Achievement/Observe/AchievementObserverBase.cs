using System;
using UnityEngine;

namespace BallzMerge.Achievement
{
    public abstract class AchievementObserverBase : IDisposable
    {
        private AchievementSettings _settings;

        protected int Count;
        protected AchievementProperty Property;

        public event Action Reached;

        public AchievementObserverBase(AchievementSettings settings)
        {
            _settings = settings;
            _settings.Completed += AchievementReached;
            Property = new AchievementProperty(settings);
            Property.Reached += OnAchievementTargetReached;
            Count = 1;
        }

        public void Dispose()
        {
            Destruct();
        }

        protected virtual void AchievementReached()
        {
            Reached?.Invoke();
            Debug.Log($"Достижение {_settings.Name} получено!");
            _settings.Completed -= AchievementReached;
        }

        protected abstract void OnAchievementTargetReached(int target, int count, int maxTarget);

        protected abstract void Destruct();

        public abstract void Construct();
    }
}