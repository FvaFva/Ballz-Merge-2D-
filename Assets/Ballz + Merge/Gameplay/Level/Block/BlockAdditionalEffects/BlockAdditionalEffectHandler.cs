using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlockAdditionalEffectHandler : CyclicBehaviour, IInitializable, ILevelFinisher
    {
        [SerializeField] private BlockAdditionalEffectSettings _settings;
        [SerializeField] private Transform _parent;
        [SerializeField] private int _countOfPreload;

        private BlocksInGame _activeBlocks;
        private Queue<BlockAdditionalEffectBase> _effectsPool;
        private List<BlockAdditionalEffectBase> _activeEffects;

        public event Action<Block, Vector2Int> BlockMoveRequired;
        public event Action<Block, int> BlockNumberChangedRequired;
        public event Action<Block> BlockDestroyRequired;

        private void Awake()
        {
            _effectsPool = new Queue<BlockAdditionalEffectBase>();
            _activeEffects = new List<BlockAdditionalEffectBase>();
        }

        public void Init()
        {
            for (int i = 0; i < _countOfPreload; i++)
                _effectsPool.Enqueue(Instantiate(_settings.GetPrefab(), transform));
        }

        public void ConnectActiveBlocks(BlocksInGame activeBlocks)
        {
            _activeBlocks = activeBlocks;
        }

        public void FinishLevel()
        {
            foreach (var effect in _activeEffects)
                effect.Deactivate();
        }

        public void HandleEvent(BlockAdditionalEffectEventProperty property)
        {
            foreach (var effect in _activeEffects.ToArray())
                effect.HandleEvent(property);
        }

        public void HandleWave(IEnumerable<Block> wave)
        {
            foreach (var activeEffect in _activeEffects.ToArray())
                activeEffect.HandleWave();

            if (wave.Count() == 0)
                return;

            if (_effectsPool.TryDequeue(out BlockAdditionalEffectBase effect) == false)
                effect = Instantiate(_settings.GetPrefab(), transform);

            _activeEffects.Add(effect);
            UpdateEffectSubscription(effect, true);
            Block block = wave.ToArray()[UnityEngine.Random.Range(0, wave.Count())];
            effect.Init(block, _activeBlocks);
        }

        private void UpdateEffectSubscription(BlockAdditionalEffectBase effect, bool isActive)
        {
            if (effect == null)
                return;

            if (isActive)
            {
                effect.BlockDestroyed += OnBlockDestroyed;
                effect.BlockMoved += OnBlockMoved;
                effect.NumberChanged += OnRequiredBlockNumberChanged;
                effect.Removed += OnAdditionalEffectDeactivate;
            }
            else
            {
                effect.BlockDestroyed -= OnBlockDestroyed;
                effect.BlockMoved -= OnBlockMoved;
                effect.NumberChanged -= OnRequiredBlockNumberChanged;
                effect.Removed -= OnAdditionalEffectDeactivate;
            }
        }

        private void OnAdditionalEffectDeactivate(BlockAdditionalEffectBase blockAdditionalEffect)
        {
            _effectsPool.Enqueue(blockAdditionalEffect);
            _activeEffects.Remove(blockAdditionalEffect);
            UpdateEffectSubscription(blockAdditionalEffect, false);
        }

        private void OnRequiredBlockNumberChanged(Block block, int count)
        {
            BlockNumberChangedRequired?.Invoke(block, count);
        }

        private void OnBlockDestroyed(Block block)
        {
            BlockDestroyRequired?.Invoke(block);
        }

        private void OnBlockMoved(Block block, Vector2Int direction)
        {
            BlockMoveRequired?.Invoke(block, direction);
        }
    }
}
