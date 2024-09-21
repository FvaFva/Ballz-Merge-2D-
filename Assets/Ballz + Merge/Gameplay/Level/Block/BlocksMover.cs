using System;
using System.Collections.Generic;
using System.Linq;
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
            if (block.Number == 0)
                return;

            ChangedCellActivity?.Invoke(block.GridPosition, false);
            block.CameToNewCell += OnBlockMoved;
            _inMove.Add(block);
            block.Move(direction);
            ChangedCellActivity?.Invoke(block.GridPosition, true);
        }

        public void ProcessDeleteBlock(Block block)
        {
            if(_inMove.Contains(block))
            {
                _inMove.Remove(block);
                block.CameToNewCell -= OnBlockMoved;
            }
        }

        public void Merge(Block block1, Block block2)
        {
            ChangedCellActivity?.Invoke(block1.GridPosition, false);
            ChangedCellActivity?.Invoke(block2.GridPosition, false);
            ProcessDeleteBlock(block1);
            ProcessDeleteBlock(block2);
            block1.Merge(block2.WorldPosition);
            block2.Merge(block1.WorldPosition);
        }

        public bool CheckCorrectRestPosition()
        {
            foreach (Block block in _inMove.Where(b => b.IsInMove == false).ToArray())
                ProcessDeleteBlock(block);
            
            return _inMove.Count == 0;
        }

        public void MoveAllDirection(IEnumerable<Block> movedBlocks, Vector2Int direction)
        {
            foreach (Block block in movedBlocks)
                Move(block, direction);
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