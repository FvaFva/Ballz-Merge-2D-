using BallzMerge.Gameplay.BlockSpace;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPool : CyclicBehavior, IInitializable
{
    [SerializeField] private List<BaseEffect> _prefabs;
    [SerializeField] private Transform _effectParent;
    [SerializeField] private int _countPreload;

    private Dictionary<BaseEffect, Queue<BaseEffect>> _effects = new Dictionary<BaseEffect, Queue<BaseEffect>>();

    public void Init()
    {
        for (int i = 0; i < _prefabs.Count; i++)
        {
            _effects.Add(_prefabs[i], new Queue<BaseEffect>());

            for (int j = 0; j < _countPreload; j++)
            {
                BaseEffect effect = Generate(_prefabs[i]);
                _effects[_prefabs[i]].Enqueue(effect);
            }
        }
    }

    public void SpawnEffect(BlockAdditionalEffectEvents currentEvent, Vector3 position)
    {
        foreach (KeyValuePair<BaseEffect, Queue<BaseEffect>> effect in _effects)
        {
            if (effect.Key.ResponsibleEvent == currentEvent)
            {
                if (effect.Value.TryDequeue(out BaseEffect result) == false)
                    result = Generate(effect.Key);

                result.Played += OnPlayed;
                result.Play(position);
            }
        }
    }

    private void OnPlayed(BaseEffect newEffect)
    {
        foreach (KeyValuePair<BaseEffect, Queue<BaseEffect>> effect in _effects)
            if (effect.Key.ResponsibleEvent == newEffect.ResponsibleEvent)
                effect.Value.Enqueue(newEffect);

        newEffect.Played -= OnPlayed;
    }

    private BaseEffect Generate(BaseEffect effect)
    {
        effect = Instantiate(effect);
        effect.Initialize(_effectParent);
        return effect;
    }
}
