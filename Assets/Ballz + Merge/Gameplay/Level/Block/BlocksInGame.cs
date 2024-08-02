using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlocksInGame
{
    private List<Block> _blocks = new List<Block>();

    public IEnumerable<Block> Items => _blocks;
    public event Action<Vector2Int, bool> ChangedCellActivity;

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
            ChangedCellActivity?.Invoke(block.GridPosition, true);
        }
    }

    public void Remove(Block block)
    {
        if (_blocks.Contains(block) == false) 
            return;

        _blocks.Remove(block);
        ChangedCellActivity?.Invoke(block.GridPosition, false);
    }

    public void Clear()
    {
        for(int i = _blocks.Count - 1; i >= 0; i--)
        {
            _blocks[i].Deactivate();
            _blocks.Remove(_blocks[i]);
        }
    }
}