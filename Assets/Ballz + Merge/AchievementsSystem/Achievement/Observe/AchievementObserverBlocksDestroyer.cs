using BallzMerge.Gameplay.BlockSpace;
using UnityEngine;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementObserverBlocksDestroyer : AchievementObserverBase
    {
        [Inject] private BlocksInGame _blocks;

        public AchievementObserverBlocksDestroyer(AchievementSettings settings) : base(settings)
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

        protected override void OnAchievementTargetReached(int target, int count, int maxTarget)
        {
            Debug.Log($"Вы уничтожили {count} блоков и достигли {target} этапа из {maxTarget}");
        }
    }
}