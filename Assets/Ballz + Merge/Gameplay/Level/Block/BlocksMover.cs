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

        public IEnumerable MoveAll(IEnumerable<Block> blocks, Vector2Int direction, Action callBack)
        {
            _onComeAllBlocks = callBack;

            foreach (var block in blocks.OrderByDescending(block => block.ID))
            {
                if(block.Move(direction, true))
                {
                    block.CameToNewCell += OnCome;
                    block.Deactivated += OnCome;
                    _blocksInMove.Add(block);
                }

                yield return null;
            }

            if (_blocksInMove.Count == 0)
                _onComeAllBlocks();
        }

        public bool IsFree(Vector2Int position)
        {
            if(_grid.IsOutside(position) || IsCollisionBlock(position))
                return false;
            return true;
        }

        private bool IsWrongDirection(Vector2Int direction) => direction == WrongDirection;

        private bool IsCollisionBlock(Vector2Int nextPosition) => _activeBlocks.HaveAtPosition(nextPosition);

        private void OnCome(Block block)
        {
            block.CameToNewCell -= OnCome;
            block.Deactivated -= OnCome;
            _blocksInMove.TryRemove(block);

            if (_blocksInMove.Count == 0)
                _onComeAllBlocks();
        }
    }
}
