using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksMover
    {
        private List<Block> _inMove = new List<Block>();

        public event Action<Block> BlockMoved;
        public event Action<Vector2Int, bool> ChangedCellActivity;

        public void Move(Block block, Vector2Int direction)
        {
            ChangedCellActivity?.Invoke(block.GridPosition, false);
            block.CameToNewCell += OnBlockMoved;
            _inMove.Add(block);
            block.Move(direction);
            ChangedCellActivity?.Invoke(block.GridPosition, true);
        }

        public void Merge(Block block1, Block block2)
        {
            ChangedCellActivity?.Invoke(block1.GridPosition, false);
            ChangedCellActivity?.Invoke(block2.GridPosition, false);
            block1.Merge(block2.WorldPosition);
            block2.Merge(block1.WorldPosition);
        }

        public void MoveAllDawn(IEnumerable<Block> movedBlocks)
        {
            foreach (Block block in movedBlocks)
                Move(block, Vector2Int.down);
        }

        public void Clear()
        {
            foreach (Block block in _inMove)
                block.CameToNewCell -= OnBlockMoved;

            _inMove.Clear();
        }

        private void OnBlockMoved(Block block)
        {
            _inMove.Remove(block);
            block.CameToNewCell -= OnBlockMoved;
            BlockMoved?.Invoke(block);
        }
    }
}