using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
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
        private DropRarity _rarity;

        [Inject]
        public BlockMagneticObserver(Ball ball, BlocksInGame blocks)
        {
            _collisionHandler = ball.GetBallComponent<BallCollisionHandler>();
            _blocks = blocks;
        }

        public void Dispose() => Clear();

        public void Activate(BallVolumeHitData hitData, DropRarity rarity)
        {
            _rarity = rarity;
            _lastBlock = hitData.Block;
            _collisionHandler.NonBlockHit += Clear;
            _blocks.BlockHit += OnBlockHit;
        }

        public void OnBlockHit(Block hitBlock, Vector2Int direction)
        {
            bool isBlocksCorrect = _lastBlock != null && _lastBlock != hitBlock;

            if (isBlocksCorrect && IsDifferenceNumberLessRarity(hitBlock))
                _lastBlock.Move(hitBlock.GridPosition - direction - _lastBlock.GridPosition);

            Clear();
        }

        private void Clear()
        {
            _rarity = default;
            _lastBlock = default;
            _collisionHandler.NonBlockHit -= Clear;
            _blocks.BlockHit -= OnBlockHit;
        }

        private bool IsDifferenceNumberLessRarity(Block hitBlock) => hitBlock.Number - _lastBlock.Number <= _rarity.Weight;
    }
}
