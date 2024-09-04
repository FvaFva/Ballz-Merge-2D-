using System;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public abstract class BlockAdditionalEffectBase : MonoBehaviour
    {
        protected Block Current { get; private set; }
        protected bool IsActive;

        public event Action<Block, Vector2Int> BlockMoved;
        public event Action<Block, int, bool> NumberChanged;
        public event Action<Block> BlockDestroyed;
        public event Action<BlockAdditionalEffectBase> Removed;

        private void Awake()
        {
            Deactivate();
        }

        public abstract void HandleWave();

        public abstract void HandleEvent(BlockAdditionalEffectEventProperty property);

        public void Init(Block targetBlock, BlocksInGame blocks)
        {
            Current = targetBlock;

            if (TryInit(blocks) == false)
                Deactivate();
            else
                gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            Current = null;
            Removed?.Invoke(this);
            gameObject.SetActive(false);
        }

        protected void InvokeActionBlockMoved(Block block, Vector2Int direction)
        {
            BlockMoved?.Invoke(block, direction);
        }

        protected void InvokeActionNumberChanged(Block block, int count, bool isEffect)
        {
            if (isEffect)
                NumberChanged?.Invoke(block, count, true);
            else
                NumberChanged?.Invoke(block, count, false);
        }

        protected void InvokeActionBlockDestroyed(Block block)
        {
            BlockDestroyed?.Invoke(block);
        }

        protected abstract bool TryInit(BlocksInGame blocks);
    }
}
