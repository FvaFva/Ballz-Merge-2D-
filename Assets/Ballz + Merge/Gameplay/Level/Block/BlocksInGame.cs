using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksInGame: IDisposable
    {
        private readonly List<Block> _blocks = new List<Block>();
        private readonly Vector2Int[] AllSides = new Vector2Int[4] { Vector2Int.right, Vector2Int.left, Vector2Int.down, Vector2Int.up };

        public IEnumerable<Block> Blocks => _blocks;

        public event Action<Block> BlockRemoved;
        public event Action<Block, Vector2Int> BlockHit;
        public event Action<Block, Block> BlocksMerged;

        public void Dispose() => Clear();

        public Block GetAtPosition(Vector2Int position)
        {
            return _blocks.Where(b => b.GridPosition == position).FirstOrDefault();
        }

        public bool TryDeactivateUnderLine(int y)
        {
            var underLiners = _blocks.Where(block => block.GridPosition.y <= y);

            if (underLiners.Any())
            {
                foreach (var underLiner in underLiners.ToList())
                    underLiner.Deactivate();

                return true;
            }

            return false;
        }

        public void AddBlocks(Block block)
        {
            if (_blocks.Contains(block))
                return;

            _blocks.Add(block);
            UpdateSubscribeForBlock(block, true);
        }

        public void Clear()
        {
            for (int i = _blocks.Count - 1; i >= 0; i--)
                _blocks[i].Deactivate();
        }

        public Block GetRandomBlock(Block selfExcluding = null, bool isWithoutEffectSelection = false)
        {
            var otherBlocks = _blocks
                .Where(block => block != selfExcluding && (isWithoutEffectSelection || block.IsWithEffect == false));

            if (otherBlocks.Any())
                return otherBlocks.ToArray()[UnityEngine.Random.Range(0, otherBlocks.Count())];
            else
                return null;
        }

        private void Remove(Block block)
        {
            if (_blocks.Contains(block))
                _blocks.Remove(block);

            UpdateSubscribeForBlock(block, false);
            BlockRemoved?.Invoke(block);
        }

        private void OnBlockHit(Block block, Vector2Int direction)
        {
            if(TryMergeCell(block, direction) == false)
                BlockHit?.Invoke(block, direction);
        }

        private void OnBlockCameNewPosition(Block block)
        {
            foreach (Vector2Int direction in AllSides)
            {
                if (TryMergeCell(block, direction))
                    return;
            }
        }

        private void MergeBlocks(Block firstBlock, Block secondBlock)
        {
            firstBlock.Merge(secondBlock.WorldPosition);
            secondBlock.Merge(firstBlock.WorldPosition);
            BlocksMerged?.Invoke(firstBlock, secondBlock);
        }

        private bool TryMergeCell(Block block, Vector2Int direction)
        {
            Block blockInNextCell = GetAtPosition(block.GridPosition + direction);

            if (blockInNextCell != null && blockInNextCell.Number == block.Number)
            {
                MergeBlocks(block, blockInNextCell);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateSubscribeForBlock(Block block, bool subscribe)
        {
            if (subscribe)
            {
                block.Hit += OnBlockHit;
                block.Freed += Remove;
                block.CameToNewCell += OnBlockCameNewPosition;
            }
            else
            {
                block.Hit -= OnBlockHit;
                block.Freed -= Remove;
                block.CameToNewCell -= OnBlockCameNewPosition;
            }
        }
    }
}