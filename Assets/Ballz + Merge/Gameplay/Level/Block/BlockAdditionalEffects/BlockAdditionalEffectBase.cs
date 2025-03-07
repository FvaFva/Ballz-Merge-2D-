using System;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public abstract class BlockAdditionalEffectBase : MonoBehaviour
    {
        protected Block Current { get; private set; }
        protected bool IsActive { get; private set; }
        protected AdditionalEffectsPool EffectsPool { get; private set; }
        protected BlocksInGame ActiveBlocks { get; private set; }
        protected BlocksMover Mover {  get; private set; }

        private Action UpdateHandler = () => { };

        public event Action<BlockAdditionalEffectBase> Removed;

        private void Awake()
        {
            Deactivate();
        }

        private void FixedUpdate()
        {
            UpdateHandler();
        }

        public BlockAdditionalEffectBase Init(BlocksInGame blocks, AdditionalEffectsPool effectsPool, BlocksMover mover)
        {
            EffectsPool = effectsPool;
            ActiveBlocks = blocks;
            Mover = mover;
            Init();
            return this;
        }

        public abstract void HandleWave();

        public void Activate(Block targetBlock)
        {
            Current = targetBlock;
            IsActive = true;
            Current.Freed += OnBlockDestroy;

            if (TryActivate())
            {
                UpdateHandler = HandleUpdate;
                gameObject.SetActive(true);
            }
            else
            {
                Deactivate();
            }
        }

        public void Deactivate()
        {
            HandleDeactivate();
            UpdateHandler = () => { };
            Current = null;
            IsActive = false;
            gameObject.SetActive(false);
            Removed?.Invoke(this);
        }

        protected abstract bool TryActivate();
        protected abstract void HandleUpdate();
        protected abstract void Init();
        protected abstract void HandleDeactivate();

        private void OnBlockDestroy(Block block)
        {
            block.Freed -= OnBlockDestroy;
            Deactivate();
        }
    }
}
