using BallzMerge.Gameplay.BlockSpace;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalEffectsPool : CyclicBehavior, IInitializable
{
    [SerializeField] private List<AdditionalEffectBase> _prefabs;
    [SerializeField] private Transform _effectParent;
    [SerializeField] private int _countPreload;

    private Dictionary<AdditionalEffectBase, Queue<AdditionalEffectBase>> _effects = new Dictionary<AdditionalEffectBase, Queue<AdditionalEffectBase>>();

    public void Init()
    {
        for (int i = 0; i < _prefabs.Count; i++)
        {
            _effects.Add(_prefabs[i], new Queue<AdditionalEffectBase>());

            for (int j = 0; j < _countPreload; j++)
            {
                AdditionalEffectBase effect = Generate(_prefabs[i]);
                _effects[_prefabs[i]].Enqueue(effect);
            }
        }
    }

    public void SpawnEffect(BlockAdditionalEffectEvents currentEvent, Vector3 position)
    {
        foreach (KeyValuePair<AdditionalEffectBase, Queue<AdditionalEffectBase>> effect in _effects)
        {
            if (effect.Key.ResponsibleEvent == currentEvent)
            {
                if (effect.Value.TryDequeue(out AdditionalEffectBase result) == false)
                    result = Generate(effect.Key);

                result.Played += OnPlayed;
                result.Play(position);
            }
        }
    }

    private void OnPlayed(AdditionalEffectBase newEffect)
    {
        foreach (KeyValuePair<AdditionalEffectBase, Queue<AdditionalEffectBase>> effect in _effects)
            if (effect.Key.ResponsibleEvent == newEffect.ResponsibleEvent)
                effect.Value.Enqueue(newEffect);

        newEffect.Played -= OnPlayed;
    }

    private AdditionalEffectBase Generate(AdditionalEffectBase effect)
    {
        effect = Instantiate(effect, _effectParent);
        return effect;
    }
}
