using BallzMerge.Gameplay.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksMover
    {
        [Inject] private GridSettings _grid;
        [Inject] private BlocksInGame _activeBlocks;

        private List<Block> _blocksInMove;
        private Action _onComeAllBlocks;

        public bool IsBlockOutside { get; private set; }

        public BlocksMover()
        {
            _blocksInMove = new List<Block>();
        }

        public IEnumerable MoveAll(IEnumerable<Block> blocks, Vector2Int direction, Action callback)
        {
            _onComeAllBlocks = callback;
            IsBlockOutside = false;

            while (_blocksInMove.Count != 0)
                yield return null;

            if (blocks.Count() == 0)
            {
                _onComeAllBlocks();
                yield break;
            }

            foreach (var block in blocks)
            {
                _blocksInMove.Add(block);
                block.Moved += OnCome;
                block.Deactivated += OnCome;
            }

            while (_blocksInMove.Count != 0)
            {
                foreach (var _ in Move(direction))
                    yield return null;
            }

            if (!IsBlockOutside)
                _onComeAllBlocks();
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

            if (block.GridPosition.y < _grid.LastRowIndex)
            {
                IsBlockOutside = true;
                block.Deactivate();
            }

            if (isFound == false)
                Debug.Log($"{block.name} was not present in blocksMoved list! Callback has been performed several times!");
        }

        private IEnumerable Move(Vector2Int direction)
        {
            IEnumerable<Block> blocksCopy = _blocksInMove.ToList();

            foreach (Block block in blocksCopy)
            {
                if (block.CanMove(direction, false) && _blocksInMove.Contains(block))
                    block.Move(direction, BlockMoveActionType.Move);

                yield return null;
            }
        }
    }
}
