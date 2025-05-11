using System;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public abstract class BlockAdditionalEffectBase : MonoBehaviour
    {
        public int ID {get; private set;}
        public Block Current { get; private set; }
        public Block ConnectBlock { get; private set; }
        protected bool IsActive { get; private set; }
        protected AdditionalEffectsPool EffectsPool { get; private set; }
        protected BlocksInGame ActiveBlocks { get; private set; }

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

        public BlockAdditionalEffectBase Init(int id, BlocksInGame blocks, AdditionalEffectsPool effectsPool)
        {
            ID = id;
            name = $"Effect {ID}";
            EffectsPool = effectsPool;
            ActiveBlocks = blocks;
            Init();
            return this;
        }

        public abstract void HandleWave();

        public void Activate(Block targetBlock, Block connectBlock = null)
        {
            Current = targetBlock;
            IsActive = true;
            Current.Deactivated += OnBlockDestroy;

            if (connectBlock != null)
                SetConnectBlock(connectBlock);

            if (TryActivate())
            {
                gameObject.SetActive(true);
                HandleUpdate();
                UpdateHandler = HandleUpdate;
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

        protected void SetConnectBlock()
        {
            ConnectBlock = ConnectBlock != null ? ConnectBlock : ActiveBlocks.GetRandomBlock(Current);
        }

        private void SetConnectBlock(Block block)
        {
            ConnectBlock = block;
        }

        private void OnBlockDestroy(Block block)
        {
            block.Deactivated -= OnBlockDestroy;
            Deactivate();
        }
    }
}
