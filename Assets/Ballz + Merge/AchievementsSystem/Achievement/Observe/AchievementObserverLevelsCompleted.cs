using BallzMerge.Data;
using System.Collections.Generic;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementObserverLevelsCompleted : AchievementObserverBase
    {
        [Inject] private DataBaseSource _data;

        private List<int> _completedLevels;
        private int _currentLevelID;

        public AchievementObserverLevelsCompleted(AchievementSettings settings, AchievementPointsStep pointsStep) : base(settings, pointsStep)
        {
        }

        public override void Construct()
        {
            _completedLevels = _data.History.GetCompleted();
        }

        protected override void Destruct()
        {
            
        }

        public void SetCurrentLevelID(int levelID)
        {
            _currentLevelID = levelID;
        }

        public void OnLevelCompleted()
        {
            if (_completedLevels.Contains(_currentLevelID) == false)
                Property.Apply(Count);
        }
    }
}
