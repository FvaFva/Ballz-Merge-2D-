using BallzMerge.Gameplay.BlockSpace;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementObserverBandlerCompleted : AchievementObserverBase
    {
        [Inject] private BlocksBinder _blocksBus;

        private int _bandlersCount;

        public AchievementObserverBandlerCompleted(AchievementSettings settings, AchievementPointsStep pointsStep) : base(settings, pointsStep)
        {
        }

        public override void Construct()
        {
        }

        protected override void Destruct()
        {
        }

        public void OnBandlerCompleted()
        {
            if (_blocksBus.EffectHandler.EffectsCount.ContainsKey(BlockAdditionalEffectType.BlockBandler))
            {
                _bandlersCount = _blocksBus.EffectHandler.EffectsCount[BlockAdditionalEffectType.BlockBandler];

                if (_bandlersCount >= Property.PointsStep.Points)
                    Property.Set(_bandlersCount);
            }
        }
    }
}
