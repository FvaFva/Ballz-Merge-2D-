using BallzMerge.Gameplay.BlockSpace;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementObserverWaveSpawned : AchievementObserverBase
    {
        [Inject] private BlocksBinder _blocksBus;

        private int _wavesLeft;

        public AchievementObserverWaveSpawned(AchievementSettings settings, AchievementPointsStep pointsStep) : base(settings, pointsStep)
        {
        }

        public override void Construct()
        {
            _blocksBus.WaveSpawned += OnWaveSpawned;
        }

        protected override void Destruct()
        {
            _blocksBus.WaveSpawned -= OnWaveSpawned;
        }

        private void OnWaveSpawned()
        {
            _wavesLeft += Count;

            if (_wavesLeft > Property.PointsStep.Points)
                Property.Apply(Count);
        }
    }
}