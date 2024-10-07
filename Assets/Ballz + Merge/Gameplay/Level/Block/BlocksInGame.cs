using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksInGame
    {
        private List<Block> _blocks = new List<Block>();

        public IList<Block> Items => _blocks;

        public event Action<Vector2Int, bool> ChangedCellActivity;
        public event Action BlocksDestroyed;

        public Block GetAtPosition(Vector2Int position)
        {
            return _blocks.Where(b => b.GridPosition == position).FirstOrDefault();
        }

        public bool TryDeactivateUnderLine(int y)
        {
            var underLiners = _blocks.Where(block => block.GridPosition.y < y);

            if (underLiners.Any())
            {
                foreach (var underLiner in underLiners.ToList())
                {
                    underLiner.Deactivate();
                    _blocks.Remove(underLiner);
                }

                return true;
            }

            return false;
        }

        public void AddBlocks(IEnumerable<Block> blocks)
        {
            foreach (Block block in blocks)
            {
                if (_blocks.Contains(block))
                    return;

                _blocks.Add(block);
                block.Destroyed += OnBlockDestroyed;
                ChangedCellActivity?.Invoke(block.GridPosition, true);
            }
        }

        public void Remove(Block block)
        {
            if (_blocks.Contains(block) == false)
                return;

            _blocks.Remove(block);
            block.Destroyed -= OnBlockDestroyed;
            ChangedCellActivity?.Invoke(block.GridPosition, false);
        }

        public void Clear()
        {
            for (int i = _blocks.Count - 1; i >= 0; i--)
            {
                _blocks[i].Deactivate();
                _blocks[i].Destroyed -= OnBlockDestroyed;
                _blocks.Remove(_blocks[i]);
            }
        }

        public Block GetRandomBlock(Block selfExcluding = null, bool isWithoutEffectSelection = false)
        {
            var otherBlocks = _blocks.Where(block => block != selfExcluding && (isWithoutEffectSelection || block.IsWithEffect == false));

            return ChooseRandomBlock(otherBlocks);
        }

        public Block GetRandomBlock()
        {
            return ChooseRandomBlock(_blocks);
        }

        private Block ChooseRandomBlock(IEnumerable<Block> blocks)
        {
            if (blocks.Any())
                return blocks.ToArray()[UnityEngine.Random.Range(0, blocks.Count())];
            else
                return null;
        }

        private void OnBlockDestroyed()
        {
            BlocksDestroyed?.Invoke();
        }
    }
}