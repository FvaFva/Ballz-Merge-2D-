using BallzMerge.Gameplay.BlockSpace;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementObserverBlocksDestroyer : AchievementObserverBase
    {
        [Inject] private BlocksInGame _blocks;

        public AchievementObserverBlocksDestroyer(AchievementSettings settings, AchievementPointsStep pointsStep) : base(settings, pointsStep)
        {
        }

        public override void Construct()
        {
            _blocks.BlockDestroyed += OnBlockDestroyed;
        }

        protected override void Destruct()
        {
            _blocks.BlockDestroyed -= OnBlockDestroyed;
        }

        private void OnBlockDestroyed()
        {
            Property.Apply(Count);
        }
    }
}