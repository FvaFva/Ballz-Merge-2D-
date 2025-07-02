using BallzMerge.Gameplay.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksMover
    {
        private readonly Vector2Int WrongDirection = Vector2Int.down;

        [Inject] private GridSettings _grid;
        [Inject] private BlocksInGame _activeBlocks;

        private List<Block> _blocksInMove;
        private Action _onComeAllBlocks;

        public BlocksMover()
        {
            _blocksInMove = new List<Block>();
        }

        public IEnumerable MoveAll(IEnumerable<Block> blocks, Vector2Int direction, Action callback)
        {
            _onComeAllBlocks = callback;

            if (blocks.Count() == 0)
            {
                _onComeAllBlocks();
                yield break;
            }

            if (direction == Vector2Int.up)
                blocks = blocks.OrderByDescending(block => block.ID);

            foreach (var block in blocks)
            {
                if (block.CanMove(direction, true))
                {
                    _blocksInMove.Add(block);
                    block.Moved += OnCome;
                    block.Deactivated += OnCome;
                }
            }

            yield return null;

            foreach (Block block in _blocksInMove.ToList())
            {
                block.Move(direction, BlockMoveActionType.Move);
                yield return null;
            }
        }

        public bool IsFree(Vector2Int position)
        {
            if (_grid.IsOutside(position) || IsCollisionBlock(position))
                return false;

            return true;
        }

        private bool IsCollisionBlock(Vector2Int nextPosition) => _activeBlocks.HaveAtPosition(nextPosition);

        private void OnCome(Block block)
        {
            block.Moved -= OnCome;
            block.Deactivated -= OnCome;
            bool isFound = _blocksInMove.Remove(block);

            if (isFound == false)
                Debug.Log($"{block.name} was not present in blocksMoved list! Callback worked several times!");

            if (_blocksInMove.Count == 0)
                _onComeAllBlocks();
        }
    }
}
