using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.BlockSpace;
using System;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class ConductorBetweenWaves
    {
        private Dropper _dropper;
        private BallAwaitBreaker _awaitBreaker;
        private BlocksBinder _binder;
        private Func<bool> _isComplete;

        public ConductorBetweenWaves(BallAwaitBreaker awaitBreaker, Dropper dropper, BlocksBinder binder, Func<bool> isComplete)
        {
            _dropper = dropper;
            _awaitBreaker = awaitBreaker;
            _binder = binder;
            _isComplete = isComplete;
        }

        public event Action WaveLoaded;
        public event Action GameIsLost;
        public event Action GameIsComplete;

        public void Start()
        {
            _binder.StartSpawnWave(() => { return; });
            _binder.BlocksOut += OnBlocksOut;
        }

        public void Continue()
        {
            ProcessBinder();
        }

        private void ProcessBinder()
        {
            _binder.StartMoveAllBlocks(Vector2Int.down, AfterMoveBlock);
        }

        private void AfterMoveBlock()
        {
            if (_isComplete())
            {
                _binder.BlocksOut -= OnBlocksOut;
                GameIsComplete?.Invoke();
                return;
            }

            _dropper.UpdateWave();
            _binder.StartSpawnWave(ProcessDropper);
        }

        private void ProcessDropper()
        {
            if (_dropper.IsReadyToDrop)
                _dropper.ShowDrop(ProcessDropper);
            else
                ProcessBall();
        }

        public void ProcessBall()
        {
            _awaitBreaker.Break();
            WaveLoaded?.Invoke();
        }

        private void OnBlocksOut()
        {
            _binder.BlocksOut -= OnBlocksOut;
            GameIsLost?.Invoke();
        }
    }
}
