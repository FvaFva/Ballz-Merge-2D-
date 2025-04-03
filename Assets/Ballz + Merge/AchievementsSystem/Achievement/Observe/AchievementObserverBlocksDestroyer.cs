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
            _blocks.BlockRemoved += OnBlockDestroyed;
        }

        protected override void Destruct()
        {
            _blocks.BlockRemoved -= OnBlockDestroyed;
        }

        private void OnBlockDestroyed(Block block)
        {
            Property.Apply(Count);
        }
    }
}