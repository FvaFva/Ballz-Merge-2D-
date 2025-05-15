using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlockAdditionalEffectHandler : CyclicBehavior, ILevelSaver, ILevelLoader, ILevelFinisher, IWaveUpdater
    {
        private const string SavedEffects = "SavedEffects";

        [SerializeField] private BlockAdditionalEffectSettings _settings;
        [SerializeField] private AdditionalEffectsPool _effectsPool;
        [SerializeField] private Transform _parent;
        [SerializeField] private int _countOfPreload;

        private BlocksInGame _activeBlocks;
        private Dictionary<int, Queue<BlockAdditionalEffectBase>> _effects;
        private List<BlockAdditionalEffectBase> _activeEffects;
        private List<SavedEffect> _savedEffects;

        public void Init(BlocksInGame activeBlocks)
        {
            _activeBlocks = activeBlocks;
            _effects = new Dictionary<int, Queue<BlockAdditionalEffectBase>>();
            _activeEffects = new List<BlockAdditionalEffectBase>();
            BlockAdditionalEffectProperty property;

            for (int i = 1; i <= _settings.GetPropertiesCount(); i++)
            {
                _effects.Add(i, new Queue<BlockAdditionalEffectBase>());
                property = _settings.GetProperty(i);

                for (int j = 1; j <= _countOfPreload; j++)
                    _effects[property.ID].Enqueue(Instantiate(property.Prefab, transform).Init(property.ID, _activeBlocks, _effectsPool));
            }
        }

        public IDictionary<string, object> GetSavingData()
        {
            List<SavedEffect> savedEffects = new List<SavedEffect>();

            foreach (var effect in _activeEffects)
            {
                if (effect.ConnectBlock == null)
                    savedEffects.Add(new SavedEffect(effect.ID, effect.Current.ID, 0));
                else
                    savedEffects.Add(new SavedEffect(effect.ID, effect.Current.ID, effect.ConnectBlock.ID));
            }

            return new Dictionary<string, object>
            {
                { SavedEffects, savedEffects }
            };
        }

        public void Load(IDictionary<string, object> data)
        {
            _savedEffects = JsonConvert.DeserializeObject<List<SavedEffect>>(data[SavedEffects].ToString());
        }

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
            var blocksDictionary = blocks.ToDictionary(block => block.ID);

            foreach (var effect in _savedEffects)
            {
                if (!blocksDictionary.TryGetValue(effect.CurrentBlock, out var effectBlock))
                    continue;

                if (blocksDictionary.TryGetValue(effect.ConnectBlock, out var connectBlock))
                {
                    SpawnEffect(effectBlock, effect.ID, connectBlock);
                    continue;
                }

                SpawnEffect(effectBlock, effect.ID);
            }
        }

        private void SpawnEffect(Block block, int? id = null, Block connectBlock = null)
        {
            BlockAdditionalEffectProperty effectProperty;
            effectProperty = id == null ? _settings.GetProperty() : _settings.GetProperty((int)id);

            if (_effects[effectProperty.ID].TryDequeue(out BlockAdditionalEffectBase effect) == false)
                effect = Instantiate(effectProperty.Prefab, transform).Init(effectProperty.ID, _activeBlocks, _effectsPool);

            _activeEffects.Add(effect);
            UpdateEffectSubscription(effect, true);
            effect.Activate(block, connectBlock);
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
            _effects[blockAdditionalEffect.ID].Enqueue(blockAdditionalEffect);
            _activeEffects.Remove(blockAdditionalEffect);
            UpdateEffectSubscription(blockAdditionalEffect, false);
        }
    }
}