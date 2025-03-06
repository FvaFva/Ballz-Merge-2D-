using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.BlockSpace;
using System;

namespace BallzMerge.Gameplay.Level
{
    public class ConductorBetweenWaves
    {
        private Dropper _dropper;
        private BallAwaitBreaker _awaitBreaker;
        private BlocksBinder _blockBus;

        public ConductorBetweenWaves(BallAwaitBreaker awaitBreaker, Dropper dropper, BlocksBinder bus)
        {
            _dropper = dropper;
            _awaitBreaker = awaitBreaker;
            _blockBus = bus;
        }

        public event Action WaveLoaded;
        public event Action GameFinished;

        public void Start()
        {
            ProcessBus();
        }

        private void ProcessDropper() 
        {
            _dropper.UpdateWave();

            if (_dropper.IsReadyToDrop)
                _dropper.ShowDrop(ProcessBall);
            else
                ProcessBall();
        }

        private void ProcessBus()
        {
            if (_blockBus.TryFinish())
                GameFinished?.Invoke();
            else
                _blockBus.StartSpawnWave(ProcessDropper);
        }

        private void ProcessBall()
        {
            _awaitBreaker.Break();
            WaveLoaded?.Invoke();
        }
    }
}
