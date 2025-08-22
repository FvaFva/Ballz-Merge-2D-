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

        public ConductorBetweenWaves(BallAwaitBreaker awaitBreaker, Dropper dropper, BlocksBinder binder)
        {
            _dropper = dropper;
            _awaitBreaker = awaitBreaker;
            _binder = binder;
        }

        public event Action WaveLoaded;
        public event Action GameIsLost;

        public void Start()
        {
            _binder.StartSpawnWave(() => { return; });
        }

        public void Continue()
        {
            ProcessBinder();
        }

        private void ProcessBinder()
        {
            if (_binder.TryFinish())
            {
                GameIsLost?.Invoke();
                return;
            }

            _binder.StartMoveAllBlocks(Vector2Int.down, AfterMoveBlock);
        }

        private void AfterMoveBlock()
        {
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
    }
}
