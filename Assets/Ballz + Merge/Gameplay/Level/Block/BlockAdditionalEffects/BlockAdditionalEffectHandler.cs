using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlockAdditionalEffectHandler : CyclicBehavior, ISaveDependedObject, ILevelFinisher, IWaveUpdater
    {
        [SerializeField] private BlockAdditionalEffectSettings _settings;
        [SerializeField] private AdditionalEffectsPool _effectsPool;
        [SerializeField] private Transform _parent;
        [SerializeField] private int _countOfPreload;

        private BlocksInGame _activeBlocks;
        private Dictionary<BlockAdditionalEffectType, Queue<BlockAdditionalEffectBase>> _effects;
        private Dictionary<BlockAdditionalEffectType, int> _effectsCount;
        private List<BlockAdditionalEffectBase> _activeEffects;
        private List<SavedBlockEffect> _savedEffects;

        public IReadOnlyDictionary<BlockAdditionalEffectType, int> EffectsCount => _effectsCount;

        public void Init(BlocksInGame activeBlocks)
        {
            _activeBlocks = activeBlocks;
            _effects = new Dictionary<BlockAdditionalEffectType, Queue<BlockAdditionalEffectBase>>();
            _effectsCount = new Dictionary<BlockAdditionalEffectType, int>();
            _activeEffects = new List<BlockAdditionalEffectBase>();
            BlockAdditionalEffectProperty property;

            for (int i = 1; i <= _settings.GetPropertiesCount(); i++)
            {
                property = _settings.GetProperty(i);

                if (property.Equals(default(BlockAdditionalEffectProperty)))
                    continue;

                _effects.Add(property.Type, new Queue<BlockAdditionalEffectBase>());

                for (int j = 1; j <= _countOfPreload; j++)
                    _effects[property.Type].Enqueue(Instantiate(property.Prefab, transform).Init(property.Type, _activeBlocks, _effectsPool));
            }
        }

        public void Save(SaveDataContainer save)
        {
            foreach (var effect in _activeEffects)
            {
                if (effect.ConnectBlock == null)
                    save.BlockEffects.Add(new SavedBlockEffect(effect.Type.ToString(), effect.Current.ID, null));
                else
                    save.BlockEffects.Add(new SavedBlockEffect(effect.Type.ToString(), effect.Current.ID, effect.ConnectBlock.ID));
            }
        }

        public void Load(SaveDataContainer save) => _savedEffects = save.BlockEffects.ToList();

        public void FinishLevel()
        {
            foreach (var effect in _activeEffects.ToArray())
                effect.Deactivate();
        }

        public void UpdateWave()
        {
            foreach (var activeEffect in _activeEffects.ToArray())
                activeEffect.HandleWave();
        }

        public void HandleWave(IEnumerable<Block> wave)
        {
            if (wave.Count() == 0 || _settings.ChanceToGetPrefab() == false)
                return;

            Block block = wave.ToList().TakeRandom();
            SpawnEffect(block);
        }

        public void LoadEffects(IEnumerable<Block> blocks)
        {
            Dictionary<int, Block> blocksDictionary = blocks.ToDictionary(block => block.ID);

            foreach (SavedBlockEffect effect in _savedEffects)
            {
                if (!blocksDictionary.TryGetValue(effect.EffectBlock, out Block effectBlock))
                    continue;

                if (blocksDictionary.TryGetValue((int)effect.ConnectBlock, out Block connectBlock))
                {
                    SpawnEffect(effectBlock, (int)Enum.Parse<BlockAdditionalEffectType>(effect.Name), connectBlock);
                    continue;
                }

                SpawnEffect(effectBlock, (int)Enum.Parse<BlockAdditionalEffectType>(effect.Name));
            }
        }

        private void SpawnEffect(Block block, int? id = null, Block connectBlock = null)
        {
            BlockAdditionalEffectProperty effectProperty;
            effectProperty = id == null ? _settings.GetProperty() : _settings.GetProperty((int)id);

            if (effectProperty.Equals(default(BlockAdditionalEffectProperty)))
                return;

            if (_effects[effectProperty.Type].TryDequeue(out BlockAdditionalEffectBase effect) == false)
                effect = Instantiate(effectProperty.Prefab, transform).Init(effectProperty.Type, _activeBlocks, _effectsPool);

            _activeEffects.Add(effect);

            AddAchievementPoint(effectProperty);

            UpdateEffectSubscription(effect, true);
            effect.Activate(block, connectBlock);
        }

        private void AddAchievementPoint(BlockAdditionalEffectProperty effectProperty)
        {
            if (_effectsCount.ContainsKey(effectProperty.Type))
            {
                int count = _activeEffects.Select(activeEffect => activeEffect.Type == effectProperty.Type).Count();

                while (count > _effectsCount[effectProperty.Type])
                    _effectsCount[effectProperty.Type]++;
            }
            else
            {
                _effectsCount.Add(effectProperty.Type, 1);
            }
        }

        private void UpdateEffectSubscription(BlockAdditionalEffectBase effect, bool isActive)
        {
            if (effect == null)
                return;

            if (isActive)
                effect.Removed += OnAdditionalEffectDeactivate;
            else
                effect.Removed -= OnAdditionalEffectDeactivate;
        }

        private void OnAdditionalEffectDeactivate(BlockAdditionalEffectBase blockAdditionalEffect)
        {
            _effects[blockAdditionalEffect.Type].Enqueue(blockAdditionalEffect);
            _activeEffects.Remove(blockAdditionalEffect);
            UpdateEffectSubscription(blockAdditionalEffect, false);
        }
    }
}