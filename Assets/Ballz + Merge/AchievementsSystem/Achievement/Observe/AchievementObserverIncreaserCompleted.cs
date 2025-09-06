using BallzMerge.Data;
using BallzMerge.Gameplay.BlockSpace;
using System.Collections.Generic;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementObserverIncreaserCompleted : AchievementObserverBase
    {
        [Inject] private BlocksBinder _blocksBus;

        private int _increasersCount;

        public AchievementObserverIncreaserCompleted(AchievementSettings settings, AchievementPointsStep pointsStep) : base(settings, pointsStep)
        {
        }

        public override void Construct()
        {
        }

        protected override void Destruct()
        {
        }

        public void OnIncreaserCompleted()
        {
            _increasersCount = _blocksBus.EffectHandler.EffectsCount[BlockAdditionalEffectType.BlockIncreaser];

            if (_increasersCount >= Property.PointsStep.Points)
                Property.Apply(_increasersCount);
        }
    }
}
