using BallzMerge.Gameplay.Level;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksMover
    {
        private readonly Vector2Int WrongDirection = Vector2Int.down;

        [Inject] private GridSettings _grid;
        [Inject] private BlocksInGame _activeBlocks;

        public void Try(Block block, Vector2Int direction)
        {
            Vector2Int nextPosition = block.GridPosition + direction;

            if (IsWrongDirection(direction) || _grid.IsOutside(nextPosition) || IsCollisionBlock(nextPosition))
                block.PlayBounceAnimation(direction);
            else
                block.Move(direction);
        }

        private bool IsWrongDirection(Vector2Int direction) => direction == WrongDirection;

        private bool IsCollisionBlock(Vector2Int nextPosition) => _activeBlocks.HaveAtPosition(nextPosition);
    }
}
