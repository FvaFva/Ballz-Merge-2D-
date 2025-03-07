using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlockAdditionalEffectHandler : CyclicBehavior, ILevelFinisher
    {
        [SerializeField] private BlockAdditionalEffectSettings _settings;
        [SerializeField] private AdditionalEffectsPool _effectsPool;
        [SerializeField] private Transform _parent;
        [SerializeField] private int _countOfPreload;

        private BlocksInGame _activeBlocks;
        private BlocksMover _mover;
        private Queue<BlockAdditionalEffectBase> _effects;
        private List<BlockAdditionalEffectBase> _activeEffects;

        public void Init(BlocksInGame activeBlocks, BlocksMover mover)
        {
            _activeBlocks = activeBlocks;
            _mover = mover;
            _effects = new Queue<BlockAdditionalEffectBase>();
            _activeEffects = new List<BlockAdditionalEffectBase>();

            for (int i = 0; i < _settings.Properties.Length; i++)
                for (int j = 0; j < _countOfPreload; j++)
                    _effects.Enqueue(Instantiate(_settings.Properties[i].Prefab, transform).Init(_activeBlocks, _effectsPool, _mover));
        }

        public void FinishLevel()
        {
            foreach (var effect in _activeEffects.ToArray())
                effect.Deactivate();
        }

        public void HandleWave(IEnumerable<Block> wave)
        {
            foreach (var activeEffect in _activeEffects.ToArray())
                activeEffect.HandleWave();

            if (wave.Count() == 0 || _settings.ChanceToGetPrefab() == false)
                return;

            if (_effects.TryDequeue(out BlockAdditionalEffectBase effect) == false)
                effect = Instantiate(_settings.GetPrefab(), transform).Init(_activeBlocks, _effectsPool, _mover);

            _activeEffects.Add(effect);
            UpdateEffectSubscription(effect, true);
            effect.Activate(wave.ToList().TakeRandom());
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
            _effects.Enqueue(blockAdditionalEffect);
            _activeEffects.Remove(blockAdditionalEffect);
            UpdateEffectSubscription(blockAdditionalEffect, false);
        }
    }
}
