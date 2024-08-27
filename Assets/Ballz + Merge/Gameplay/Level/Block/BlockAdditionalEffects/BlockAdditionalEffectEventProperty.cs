using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public struct BlockAdditionalEffectEventProperty
    {
        public BlockAdditionalEffectEvents EffectEvents;
        public Block Current;
        public int Count;
        public Vector2Int Direction;

        public BlockAdditionalEffectEventProperty(BlockAdditionalEffectEvents effectEvents, Block block, int count)
        {
            EffectEvents = effectEvents;
            Current = block;
            Count = count;
            Direction = Vector2Int.zero;
        }

        public BlockAdditionalEffectEventProperty(BlockAdditionalEffectEvents effectEvents, Block block, Vector2Int direction)
        {
            EffectEvents = effectEvents;
            Current = block;
            Count = 0;
            Direction = direction;
        }

        public BlockAdditionalEffectEventProperty(BlockAdditionalEffectEvents effectEvents, Block block)
        {
            EffectEvents = effectEvents;
            Current = block;
            Count = 0;
            Direction = Vector2Int.zero;
        }
    }
}
