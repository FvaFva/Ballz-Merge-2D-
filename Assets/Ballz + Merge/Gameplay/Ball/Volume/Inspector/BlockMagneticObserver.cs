using BallzMerge.Gameplay.BlockSpace;
using System;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BallSpace
{
    public class BlockMagneticObserver : IDisposable
    {
        private Block _lastBlock;
        private BallCollisionHandler _collisionHandler;
        private BlocksInGame _blocks;
        private int _value;
        private Action<bool> _callback;

        [Inject]
        public BlockMagneticObserver(Ball ball, BlocksInGame blocks)
        {
            _collisionHandler = ball.GetBallComponent<BallCollisionHandler>();
            _blocks = blocks;
        }

        public void Dispose() => Clear();

        public void Activate(BallVolumeHitData hitData, int value, Action<bool> callback)
        {
            _value = value;
            _lastBlock = hitData.Block;
            _collisionHandler.NonBlockHit += OnNonBlockHit;
            _blocks.BlockHit += OnBlockHit;
            _callback = callback;
        }

        public void OnBlockHit(Block hitBlock, Vector2Int direction)
        {
            bool isBlocksCorrect = _lastBlock != null && _lastBlock != hitBlock;
            bool isWent = false;
            Vector2Int moveDirection = hitBlock.GridPosition - direction - _lastBlock.GridPosition;

            if (isBlocksCorrect && IsDifferenceNumberLessRarity(hitBlock))
                isWent = _lastBlock.CanMove(moveDirection);

            if (isWent)
                _lastBlock.Move(moveDirection, BlockMoveActionType.ChangePosition);

            Clear(isWent);
        }

        private void OnNonBlockHit() => Clear();

        private void Clear(bool isWent = false)
        {
            _value = 0;
            _lastBlock = default;
            _callback?.Invoke(isWent);
            _callback = null;
            _collisionHandler.NonBlockHit -= OnNonBlockHit;
            _blocks.BlockHit -= OnBlockHit;
        }

        private bool IsDifferenceNumberLessRarity(Block hitBlock) => hitBlock.Number - _lastBlock.Number <= _value;
    }
}
