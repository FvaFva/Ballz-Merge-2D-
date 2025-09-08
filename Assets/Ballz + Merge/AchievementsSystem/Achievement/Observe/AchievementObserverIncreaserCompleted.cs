using BallzMerge.Gameplay.BlockSpace;
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
            if (_blocksBus.EffectHandler.EffectsCount.ContainsKey(BlockAdditionalEffectType.BlockIncreaser))
            {
                _increasersCount = _blocksBus.EffectHandler.EffectsCount[BlockAdditionalEffectType.BlockIncreaser];

                if (_increasersCount >= Property.PointsStep.Points)
                    Property.Set(_increasersCount);
            }
        }
    }
}
